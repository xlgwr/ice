namespace Tools.OpenExe.Exts
{ 

    public class ViewModelNotify : NotifyBase
    {

        private DynamicProxy _Model;
        public DynamicProxy Model
        {
            get
            {
                return _Model;
            }
            set
            {
                _Model = value;
                RaisePropertyChanged("Model");
            }
        }
    }
}