namespace EventMapHpViewer.Models.Settings
{
    public class TpSetting: Livet.NotificationObject
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public int Id { get; set; }
        public int SortId { get; set; }
        public string Name { get; set; }

        private decimal _Tp;
        public decimal Tp
        {
            get => this._Tp;
            set
            {
                if (this._Tp == value)
                    return;
                this._Tp = value;
                this.RaisePropertyChanged();
            }
        }

        public TpSetting() { }

        public TpSetting(int id, int sortId, string name, decimal tp = 0, int typeId = 0, string typeName = "")
        {
            this.Id = id;
            this.SortId = sortId;
            this.Name = name;
            this.Tp = tp;
            this.TypeId = typeId;
            this.TypeName = typeName;
        }
    }
}
