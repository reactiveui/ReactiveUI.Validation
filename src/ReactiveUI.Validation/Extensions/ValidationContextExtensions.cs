using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.TemplateGenerators;

namespace ReactiveUI.Validation.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="ValidationContext"/>.
    /// </summary>
    public static class ValidationContextExtensions
    {
        /// <summary>
        /// Resolve the property valuation for a specified property.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public static BasePropertyValidation<TViewModel, TProperty> ResolveFor<TViewModel, TProperty>(
            this ValidationContext context,
            Expression<Func<TViewModel, TProperty>> viewModelProperty,
            bool strict = true)
        {
            var instance = context.Validations
                .Where(p => p is BasePropertyValidation<TViewModel, TProperty>)
                .Cast<BasePropertyValidation<TViewModel, TProperty>>()
                .FirstOrDefault(v => v.ContainsProperty(viewModelProperty, strict));

            return instance;
        }
        
        /// <summary>
        /// Resolve the property valuation for a specified property.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public static IEnumerable<BasePropertyValidation<TViewModel, TProperty>> ResolveForMultiple<TViewModel, TProperty>(
            this ValidationContext context,
            Expression<Func<TViewModel, TProperty>> viewModelProperty,
            bool strict = true)
        {
            var validations = context.Validations
                .Where(p => p is BasePropertyValidation<TViewModel, TProperty>)
                .Cast<BasePropertyValidation<TViewModel, TProperty>>()
                .Where(v => v.ContainsProperty(viewModelProperty, strict));

            return validations;
        }

        /// <summary>
        /// Resolve the property valuation for two properties.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewModelProperty1"></param>
        /// <param name="viewModelProperty2"></param>
        /// <param name="strict"></param>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TProperty1"></typeparam>
        /// <typeparam name="TProperty2"></typeparam>
        /// <returns></returns>
        public static BasePropertyValidation<TViewModel, TProperty1, TProperty2> ResolveFor<TViewModel, TProperty1,
            TProperty2>(this ValidationContext context,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty1,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty2,
            bool strict = true)
        {
            var instance = context
                .Validations
                .Where(p => p is BasePropertyValidation<TViewModel, TProperty1, TProperty2>)
                .Cast<BasePropertyValidation<TViewModel, TProperty1, TProperty2>>()
                .FirstOrDefault(v =>
                    v.ContainsProperty(viewModelProperty1) && v.ContainsProperty(viewModelProperty2) &&
                    v.PropertyCount == 2);

            return instance;
        }

        /// <summary>
        /// Resolve the property valuation for three properties.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewModelProperty1"></param>
        /// <param name="viewModelProperty2"></param>
        /// <param name="viewModelProperty3"></param>
        /// <param name="strict"></param>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TProperty1"></typeparam>
        /// <typeparam name="TProperty2"></typeparam>
        /// <typeparam name="TProperty3"></typeparam>
        /// <returns></returns>
        public static BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3> ResolveFor<TViewModel,
            TProperty1, TProperty2, TProperty3>(this ValidationContext context,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty1,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty2,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty3,
            bool strict = true)
        {
            var instance = context
                .Validations
                .Where(p => p is BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3>)
                .Cast<BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3>>()
                .FirstOrDefault(v =>
                    v.ContainsProperty(viewModelProperty1) && v.ContainsProperty(viewModelProperty2) &&
                    v.ContainsProperty(viewModelProperty3) && v.PropertyCount == 3);

            return instance;
        }
    }
}