using FluentValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace EstoqueMvp.Api.Filters
{
    /// <summary>
    /// Filtro global de exceções que centraliza o tratamento de erros da API.
    /// Mapeia tipos de exceção para os códigos HTTP apropriados e registra logs de erro.
    /// Garante que nenhuma exceção não tratada vaze detalhes internos para o cliente.
    /// </summary>
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Intercepta exceções não tratadas e retorna respostas HTTP padronizadas.
        /// </summary>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            var action = actionExecutedContext.ActionContext.ActionDescriptor;
            var controller = action.ControllerDescriptor.ControllerName;
            var actionName = action.ActionName;

            // 1. Erros de Validação do FluentValidation -> HTTP 400 (Bad Request)
            if (exception is ValidationException validationException)
            {
                var erros = validationException.Errors.Select(e => e.ErrorMessage);

                Trace.TraceWarning($"[Validação] {controller}/{actionName}: {string.Join("; ", erros)}");

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new { Mensagem = "Ocorreram erros de validação.", Erros = erros });

                return;
            }

            // 2. Erros de Regra de Negócio -> HTTP 400 (Bad Request)
            if (exception is InvalidOperationException || exception is ArgumentException)
            {
                Trace.TraceWarning($"[Regra de Negócio] {controller}/{actionName}: {exception.Message}");

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new { Mensagem = exception.Message });

                return;
            }

            // 3. Recurso não encontrado -> HTTP 404 (Not Found)
            if (exception is KeyNotFoundException)
            {
                Trace.TraceWarning($"[Não Encontrado] {controller}/{actionName}: {exception.Message}");

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.NotFound,
                    new { Mensagem = exception.Message });

                return;
            }

            // 4. Falha de Login / Sem permissão -> HTTP 401 (Unauthorized)
            if (exception is UnauthorizedAccessException)
            {
                Trace.TraceWarning($"[Autenticação] {controller}/{actionName}: {exception.Message}");

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new { Mensagem = exception.Message });

                return;
            }

            // 5. Erro de Transação Abortada (Deadlock, Timeout, etc.) -> HTTP 409 (Conflict)
            if (exception is System.Transactions.TransactionAbortedException)
            {
                Trace.TraceError($"[Transação Abortada] {controller}/{actionName}: O banco de dados interrompeu a operação de forma inesperada. {exception.Message}");

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.Conflict,
                    new { Mensagem = "Conflito ao acessar o banco de dados. Por favor, tente a operação novamente em alguns instantes." });

                return;
            }

            // 6. Erro fatal / Não mapeado -> HTTP 500 (Internal Server Error)
            // Registra o erro completo para diagnóstico, mas não expõe detalhes ao cliente
            Trace.TraceError($"[Erro Interno] {controller}/{actionName}: {exception}");

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                new
                {
                    Mensagem = "Ocorreu um erro interno no servidor."
                });
        }
    }
}