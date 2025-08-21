using FluentValidation;
using Mottu.Application.Validators;
using Mottu.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;


namespace Mottu.Application.Common
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseService(INotification notification)
    {
        public virtual FluentValidation.Results.ValidationResult ValidationResult { get; protected set; }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action) => await action();

        protected virtual void Validate<TModel>(TModel model, AbstractValidator<TModel> validator)
        {
            ValidationResult = validator.Validate(model);

            if (!ValidationResult.IsValid)
            {
                foreach (var error in ValidationResult.Errors)
                {
                    notification.AddNotification(error.PropertyName, error.ErrorMessage, NotificationModel.ENotificationType.BadRequestError);
                }
                throw new ValidatorException();
            }
        }
    }
}
