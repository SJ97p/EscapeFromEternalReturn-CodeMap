//using HBDinosaur_ER_Project.InventoryLogic;
//using SingletonPattern_Scripts;

//namespace HBDinosaur_ER_Project.InventoryData
//{
//    public class InventoryItemsCarrier : Singleton<InventoryItemsCarrier>
//    {
//        public Inventory Inventory { get; private set; }
//        public Equipment Equipment { get; private set; }

//        protected override void Awake()
//        {
//            base.Awake();
//            if (Inventory == null) Inventory = new Inventory(12);
//            if (Equipment == null) Equipment = new Equipment();
//        }

//        public void Save(Inventory inventory, Equipment equipment)
//        {
//            Inventory = inventory;
//            Equipment = equipment;
//        }
//    }
//}