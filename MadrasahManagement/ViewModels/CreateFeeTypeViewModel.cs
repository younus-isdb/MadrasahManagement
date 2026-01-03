using MadrasahManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace MadrasahManagement.ViewModels
{
    public class CreateFeeTypeViewModel
    {
        [Required(ErrorMessage = "Please select a department")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Please select a class")]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Fee name is required")]
        [MaxLength(150, ErrorMessage = "Fee name cannot exceed 150 characters")]
        [Display(Name = "Fee Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please select frequency")]
        public FeeFrequency Frequency { get; set; }
    }

    public class EditFeeTypeViewModel
    {
        public int FeeTypeId { get; set; }

        [Required(ErrorMessage = "Please select a department")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Please select a class")]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Fee name is required")]
        [MaxLength(150, ErrorMessage = "Fee name cannot exceed 150 characters")]
        [Display(Name = "Fee Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please select frequency")]
        public FeeFrequency Frequency { get; set; }
    }
}
