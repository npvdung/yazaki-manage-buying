$(document).ready(function () {
    $("#customerDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/api/BillPaymentApi",
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
                    return `<a href="/BillPayment/Edit/${Id}" m-1">View | Edit</a>`;
                }
            },
            {
                "data": null,
                "name": "STT",
                "width": "50px",
                "autoWidth": true,
                "orderable": false,
                "searchable": false,
                "render": function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
           
            { "data": "billPaymentCode", "name": "billPaymentCode", "autoWidth": true },
            {
                "data": "billPaymentDate",
                "name": "billPaymentDate",
                "autoWidth": true,
                "render": function (data, type, row) {
                    // Định dạng ngày tháng kiểu Việt Nam
                    let date = new Date(data);
                    return date.toLocaleDateString("vi-VN");
                }
            },
            {
                "data": "totalAmount",
                "name": "totalAmount",
                "autoWidth": true,
                "render": function (data, type, row) {
                    // Định dạng Tiền Việt Nam Đồng
                    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(data);
                }
            }
        ],
        "lengthMenu": [[5, 10, 20, 50, 100], [5, 10, 20, 50, 100]],
        "pageLength": 5
    });
});
function DeleteEmp(id) {
    $.ajax({
        url: '/api/CategoryApi/DeleteEmp?id=' + id,
        type: 'DELETE',
        success: function (result) {
            debugger;
            // Xử lý kết quả trả về từ server (nếu cần)
            location.reload();
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi (nếu có)
            console.log(xhr.responseText);
        }
    });
    $.ajax({
        url: '/api/CategoryApi/SendMes',
        type: 'POST',
        data: { // Dữ liệu gửi lên API Controller
            // Dữ liệu của bạn
        },
        success: function (response) {
            // Xử lý phản hồi từ API Controller
            // Hiển thị thông báo thành công
            alert(response); // Hoặc sử dụng một thư viện thông báo khác
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi nếu cần
        }
    });
}
function EditEmp(id) {

}