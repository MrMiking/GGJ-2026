namespace GGJ2026
{
    public struct MaskAttachContext
    {
        public MaskInventory Inventory { get; }

        public MaskAttachContext(MaskInventory inventory)
        {
            this.Inventory = inventory;
        }
    }
}