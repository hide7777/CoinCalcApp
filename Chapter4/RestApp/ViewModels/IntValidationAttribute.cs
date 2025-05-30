﻿using System;
using System.ComponentModel.DataAnnotations;

namespace RestApp.ViewModels
{
    public class IntValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value == null) return false;
            return int.TryParse(value.ToString(), out var _);
        }
    }
}
