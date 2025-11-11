$(document).ready(function () {
    $("#customerDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/api/PurchaseContractApi",
            "type": "GET",
            "datatype": "json",
            "dataSrc": "data"
        },
        "columnDefs": [{
            "targets": [0],
            "visible": false,
            "searchable": false
        }],
        "columns": [
            { "data": "id", "name": "Id", "autoWidth": true },
            {
                "targets": 1,
                "width": "80px",
                "orderable": false,
                "render": function (data, type, row) {
                    var Id = '';
                    if (type === 'display' && data !== null) {
                        Id = row.id;
                    }
                    return `<a href="/PurchaseContract/Edit/${Id}" m-1">View | Edit</a>`;
                }
            },
            {
                "targets": 1,
                "width": "70px",
                "orderable": false,
                "render": function (data, type, row) {
                    var Id = '';
                    if (type === 'display' && data !== null) {
                        Id = row.id;
                    }
                    var status = 0;
                    if (data !== null) {
                        status = row.status;
                    }
                    if (status === 0) {
                        return `<a onclick="if (confirm('Bạn có chắc chắn muốn duyệt hợp đồng này?')) { DeleteEmp('${Id}'); }">Wait</a>`;
                    } else if (status === 1 || status === 2) {
                        return `<a>Approved</a>`;
                    } else {
                        return `<a>Reject</a>`;
                    }
                }
            },
            {
                "data": null,
                "name": "STT1",
                "width": "50px",
                "autoWidth": true,
                "orderable": false,
                "searchable": false,
                "render": function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
           
            { "data": "purchaseContractCode", "name": "purchaseContractCode", "autoWidth": true },
            { "data": "purchaseContractName", "name": "purchaseContractName", "autoWidth": true },
            { "data": "vendorName", "name": "vendorName", "autoWidth": true, "orderable": false},
            //{ "data": "totalAmountIncludeTaxAndDiscount", "name": "totalAmountIncludeTaxAndDiscount", "autoWidth": true, "orderable": false},
            {
                data: "status",
                name: "status",
                autoWidth: true,
                orderable: false,
                render: function (data, type, row) {
                    if (data === 0) {
                        return '<span class="badge badge-success">Process</span>';
                    } else if (data === 1 || data === 2) {
                        return '<span class="badge badge-secondary">Done</span>';
                    } else {
                        return '<span class="badge badge-danger ">Cancel</span>';
                    }
                }
            }

        ],
        "lengthMenu": [[5, 10, 20, 50, 100], [5, 10, 20, 50, 100]],
        "pageLength": 5
    });
});
function DeleteEmp(id) {
    $.ajax({
        url: '/api/PurchaseContractApi/Approved?id=' + id,
        type: 'POST',
        success: function (result) {
            debugger;
            var targetUrl = '/PurchaseOrder/Edit/' + result.tempId;
            window.location.href = targetUrl;
            // Xử lý kết quả trả về từ server (nếu cần)
            /*location.reload();*/
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi (nếu có)
            console.log(xhr.responseText);
        }
    });
    //$.ajax({
    //    url: '/api/CategoryApi/SendMes',
    //    type: 'POST',
    //    data: { // Dữ liệu gửi lên API Controller
    //        // Dữ liệu của bạn
    //    },
    //    success: function (response) {
    //        // Xử lý phản hồi từ API Controller
    //        // Hiển thị thông báo thành công
    //        alert(response); // Hoặc sử dụng một thư viện thông báo khác
    //    },
    //    error: function (xhr, status, error) {
    //        // Xử lý lỗi nếu cần
    //    }
    //});
}
function EditEmp(id) {

}