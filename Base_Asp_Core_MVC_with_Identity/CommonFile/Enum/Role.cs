namespace Base_Asp_Core_MVC_with_Identity.CommonFile.Enum
{
    public enum Roles
    {
        Admin,
        ManagerCategory,
        ManagerImport,
        ManagerReturn,
        ManagerStock,
        ViewReport,
        ManagerSales,
        Employee,
    }
    public enum Status
    {
        [Display(Name = "Chờ")]
        Process,
        [Display(Name = "Từ chối")]
        Rejected,
        [Display(Name = "Xác nhận")]
        Approved,
        [Display(Name = "Huỷ")]
        Cancelled
    }
    public enum ActiveStatus
    {
        Active,
        UnActive
    }

    public enum ApprodedStatus
    {
        Process,
        Success,
        Cancel
    }

    public enum ActiveVenderStatus
    {
        Active,
        Off
    }

    public enum EnumPurchaseContract
    {
        Wait,
        Approved,
        Done,
        Cancel
    }
    public enum EnumPurchaseOrder
    {
        WaitShip,
        ProcessShip,
        DoneShip,
        CancelShip,
    }
    public enum EnumShip
    {
        ProcessShip,
        DoneShip,
        CancelShip,
    }
    public enum EnumReceipt
    {
        Success,
        Fail,
    }
}
