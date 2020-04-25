using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models.ViewModels
{
    public interface IViewModel
    {
        User CurrentUser { get; set; }
    }
}
