using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.OpenExe.Exts;

namespace Tools.OpenExe.VM
{
    public class MainViewModel : NotifyBase
    {
        private ColorViewModel colorViewModel;

        public MainViewModel()
        {
            this.ColorViewModel = new ColorViewModel();
        }

        public ColorViewModel ColorViewModel
        {
            get { return this.colorViewModel; }

            private set
            {
                if (Equals(value, this.colorViewModel))
                {
                    return;
                }

                this.colorViewModel = value;
                this.OnPropertyChanged();
            }
        }
    }
}
