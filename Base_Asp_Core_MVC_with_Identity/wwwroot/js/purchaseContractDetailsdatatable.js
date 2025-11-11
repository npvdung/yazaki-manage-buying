$(document).ready(function () {
    $("#customerDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/api/ReportApi",
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                // Hiển thị tổng tiền
                $("#totalAmount").text(json.totalAmount.toLocaleString());
                return json.data;
            }
        },
        "columnDefs": [{
            "targets": [0],
            "visible": false,
            "searchable": false
        }],
        "columns": [
            { "data": "id", "name": "Id", "autoWidth": true },
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
            { "data": "productName", "name": "productName", "autoWidth": "200px" },
            { "data": "quantity", "name": "quantity", "autoWidth": true },
            { "data": "taxAmount", "name": "taxAmount", "autoWidth": true },
            { "data": "discountAmount", "name": "discountAmount", "autoWidth": true, "orderable": false },
            { "data": "totalAmount", "name": "totalAmount", "autoWidth": true },
            
        ],
        "lengthMenu": [[5, 10, 20, 50, 100], [5, 10, 20, 50, 100]],
        "pageLength": 10
    });
    //// Tìm kiếm theo ngày
    //$("#searchButton").on("click", function () {
    //    table.ajax.reload();
    //});
});