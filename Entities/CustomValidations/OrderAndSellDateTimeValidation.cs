using System.ComponentModel.DataAnnotations;

namespace StockApp.Validations
{
    public class OrderAndSellDateTimeValidation : ValidationAttribute
    {
        public DateTime MinimumDate { get; set; }
        public string DefaultErrorMessage { get; set; }= "Order date should be greater than or equal to {0}";
        public OrderAndSellDateTimeValidation(string MinimumDatestring)
        {
            MinimumDate=Convert.ToDateTime(MinimumDatestring);
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime dateTime = Convert.ToDateTime(value);
                if(dateTime > MinimumDate)
                {
                    return new ValidationResult(string.Format(ErrorMessage?? DefaultErrorMessage,MinimumDate.ToString("yyyy-MM-dd")), new string[] { nameof(validationContext.MemberName) });
                }
                return ValidationResult.Success;
            }
            return null;
        }
    }
}
