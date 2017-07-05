using System;
using System.Linq;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.ViewModels
{
    public class IDViewModel : MvxViewModel
    {
        public ID ID { get; set; }
        public List<Attribute> Attributes { get; set; }

        //TODO: For more information see: https://www.mvvmcross.com/documentation/fundamentals/navigation
        public void Init()
        {
            ID = Mvx.Resolve<ID>();
            Attributes = Mvx.Resolve<ID>().Attributes.Values.ToList();
        }

        public override void Start()
        {
            //TODO: Add starting logic here
        }
    }
}
