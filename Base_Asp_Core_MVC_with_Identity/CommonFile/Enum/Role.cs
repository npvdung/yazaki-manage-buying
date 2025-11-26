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
        [Display(Name = "Hoạt động")]
        Active,
        [Display(Name = "Ngừng hoạt động")]
        UnActive
    }

    public enum ApprodedStatus
    {
        [Display(Name = "Đang chờ")]
        Process,
        [Display(Name = "Đã duyệt")]
        Success,
        [Display(Name = "Huỷ")]
        Cancel
    }

    public enum ActiveVenderStatus
    {
        [Display(Name = "Đang hoạt động")]
        Active,
        [Display(Name = "Ngừng hoạt động")]
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
        [Display(Name = "Đã nhận")]
        Success,
        [Display(Name = "Thất bại")]
        Fail,
    }
}
