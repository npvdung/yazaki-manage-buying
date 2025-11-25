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
        [Display(Name = "Chờ duyệt")]
        Wait,
        [Display(Name = "Đã duyệt")]
        Approved,
        [Display(Name = "Hoàn thành")]
        Done,
        [Display(Name = "Đã huỷ")]
        Cancel
    }
    public enum EnumPurchaseOrder
    {
        [Display(Name = "Chờ ship")]
        WaitShip,
        [Display(Name = "Đang ship")]
        ProcessShip,
        [Display(Name = "Đã ship")]
        DoneShip,
        [Display(Name = "Huỷ ship")]
        CancelShip,
    }
    public enum EnumShip
    {
        [Display(Name = "Đang ship")]
        ProcessShip,
        [Display(Name = "Đã ship")]
        DoneShip,
        [Display(Name = "Huỷ ship")]
        CancelShip,
    }
    public enum EnumReceipt
    {
        Success,
        Fail,
    }
}
