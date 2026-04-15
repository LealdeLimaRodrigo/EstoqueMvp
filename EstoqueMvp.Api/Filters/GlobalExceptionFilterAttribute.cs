using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace EstoqueMvp.Api.Filters
{
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;

            // 1. Erros de Validação do FluentValidation -> HTTP 400 (Bad Request)
            if (exception is ValidationException validationException)
            {
                // Mensagens de erro limpas para enviar ao Front-end
                var erros = validationException.Errors.Select(e => e.ErrorMessage);

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new { Mensagem = "Ocorreram erros de validação.", Erros = erros });

                return;
            }

            // 2. Erros de Regra de Negócio -> HTTP 400 (Bad Request)
            if (exception is InvalidOperationException || exception is ArgumentException)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new { Mensagem = exception.Message });

                return;
            }

            // 3. Recurso não encontrado -> HTTP 404 (Not Found)
            if (exception is KeyNotFoundException)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.NotFound,
                    new { Mensagem = exception.Message });

                return;
            }

            // 4. Falha de Login / Sem permissão -> HTTP 401 (Unauthorized)
            if (exception is UnauthorizedAccessException)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new { Mensagem = exception.Message });

                return;
            }

            // 5. Erro fatal / Não mapeado -> HTTP 500 (Internal Server Error)
            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                new
                {
                    Mensagem = "Ocorreu um erro interno no servidor.",
                    //Detalhes = exception.Message
                });
        }
    }
}