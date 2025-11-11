$(document).ready(function () {
    $("#customerDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/api/VendorApi",
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
                    return `<a href="/Vendor/Edit/${Id}" m-1">View | Edit</a>`;
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
           
            { "data": "vendorCode", "name": "vendorCode", "autoWidth": true },
            { "data": "vendorName", "name": "vendorName", "autoWidth": true },
            { "data": "phone", "name": "phone", "autoWidth": true, "orderable": false},
            { "data": "symbol", "name": "symbol", "autoWidth": true, "orderable": false},
            {
                data: "status",
                name: "status",
                autoWidth: true,
                orderable: false,
                render: function (data, type, row) {
                    if (data === 0) {
                        return '<span class="badge badge-success">Active</span>';
                    } else if (data === 1) {
                        return '<span class="badge badge-danger">Off</span>';
                    } else {
                        return '<span class="badge badge-secondary">Unknown</span>';
                    }
                }
            }
            //{
            //    "targets": 1,
            //    "width": "50px",
            //    "orderable": false,
            //    "render": function (data, type, row) {
            //        var Id = '';
            //        if (type === 'display' && data !== null) {
            //            Id = row.id;
            //        }
            //        return `<a href="/Category/Edit/${Id}" class="btn btn-primary center-block m-1">Sửa</a>`;
            //    }
            //},

            //{
            //    "targets": 1,
            //    "width": "70px",
            //    "orderable": false,
            //    "render": function (data, type, row) {
            //        var Id = '';
            //        if (type === 'display' && data !== null) {
            //            Id = row.id;
            //        }
            //        return `<button type="button" class="btn btn-danger center-block m-1" title="Xóa thông tin này" onclick="if (confirm('Bạn có chắc chắn muốn xóa nhân viên này?')) { DeleteEmp('${Id}'); }">Xoá</button>`;
            //    }
            //},

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