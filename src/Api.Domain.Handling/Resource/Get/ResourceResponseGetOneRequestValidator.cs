﻿using FluentValidation;
using System.Net;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetOneRequestValidator : AbstractValidator<ResourceResponseGetOneRequest> {
        private List<HttpStatusCode> _acceptableCodes = new List<HttpStatusCode>() {
        HttpStatusCode.NotFound, System.Net.HttpStatusCode.NotModified, System.Net.HttpStatusCode.OK};
        public ResourceResponseGetOneRequestValidator()
        {
            RuleFor(rule => rule.StatusCode).Must(value => _acceptableCodes.Contains(value));
        }
    }


}
