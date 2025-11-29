$(document).ready(function () {
  $("#customerDatatable").DataTable({
    processing: true,
    serverSide: true,
    filter: true,
    ajax: {
      url: "/api/ShipmentRequestApi",
      type: "GET",
      datatype: "json",
      dataSrc: "data",
    },
    columnDefs: [
      {
        targets: [0],
        visible: false,
        searchable: false,
      },
    ],
    columns: [
      // 0. ID (ẩn)
      { data: "id", name: "Id", autoWidth: true },

      // 1. Xem | Sửa
      {
        data: null,
        width: "80px",
        orderable: false,
        render: function (data, type, row) {
          var Id = "";
          if (type === "display" && row && row.id) {
            Id = row.id;
          }
          return `<a href="/ShipmentRequest/Edit/${Id}" m-1">Xem | Sửa</a>`;
        },
      },

      // 2. Nhận hàng / Hoàn thành
      {
        data: null,
        width: "80px",
        orderable: false,
        render: function (data, type, row) {
          var Id = "";
          if (type === "display" && row && row.id) {
            Id = row.id;
          }
          if (row.status === "ProcessShip") {
            return `<a href="/ItemReceipt/ItemReceipt/${Id}" m-1">Nhận hàng</a>`;
          } else {
            return `<a m-1">Hoàn thành</a>`;
          }
        },
      },

      // 3. STT
      {
        data: null,
        name: "STT1",
        width: "50px",
        autoWidth: true,
        orderable: false,
        searchable: false,
        render: function (data, type, row, meta) {
          return meta.row + meta.settings._iDisplayStart + 1;
        },
      },

      // 4. Mã vận đơn
      {
        data: "shipmentRequestCode",
        name: "shipmentRequestCode",
        autoWidth: true,
      },

      // 5. Mã đặt hàng (PurchaseOrderCode)
      {
        data: "purchaseOrderCode",
        name: "purchaseOrderCode",
        autoWidth: true,
        orderable: false,
      },

      // 6. Tên hợp đồng mua hàng (PurchaseContractName)
      {
        data: "purchaseContractName",
        name: "purchaseContractName",
        autoWidth: true,
      },

      // 7. Trạng thái
      {
        data: "status",
        name: "status",
        autoWidth: true,
        orderable: false,
        render: function (data, type, row) {
          if (data === "ProcessShip") {
            return '<span class="badge badge-secondary">Đang ship</span>';
          } else if (data === "DoneShip") {
            return '<span class="badge badge-success">Đã ship</span>';
          } else {
            return '<span class="badge badge-danger">Huỷ ship</span>';
          }
        },
      },
    ],
    lengthMenu: [
      [5, 10, 20, 50, 100],
      [5, 10, 20, 50, 100],
    ],
    pageLength: 5,
  });
});

function DeleteEmp(id) {
  $.ajax({
    url: "/api/CategoryApi/DeleteEmp?id=" + id,
    type: "DELETE",
    success: function (result) {
      debugger;
      // Xử lý kết quả trả về từ server (nếu cần)
      location.reload();
    },
    error: function (xhr, status, error) {
      // Xử lý lỗi (nếu có)
      console.log(xhr.responseText);
    },
  });
  $.ajax({
    url: "/api/CategoryApi/SendMes",
    type: "POST",
    data: {
      // Dữ liệu gửi lên API Controller
      // Dữ liệu của bạn
    },
    success: function (response) {
      // Xử lý phản hồi từ API Controller
      // Hiển thị thông báo thành công
      alert(response); // Hoặc sử dụng một thư viện thông báo khác
    },
    error: function (xhr, status, error) {
      // Xử lý lỗi nếu cần
    },
  });
}

function EditEmp(id) {}
