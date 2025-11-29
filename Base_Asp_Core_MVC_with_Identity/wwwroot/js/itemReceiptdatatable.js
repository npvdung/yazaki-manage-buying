$(document).ready(function () {
  $("#customerDatatable").DataTable({
    processing: true,
    serverSide: true,
    filter: true,
    ajax: {
      url: "/api/ItemReceiptApi",
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
      // 0. Id (ẩn)
      { data: "id", name: "Id", autoWidth: true },

      // 1. Xem | Sửa
      {
        data: null,
        width: "80px",
        orderable: false,
        render: function (data, type, row) {
          var Id = "";
          if (type === "display" && data !== null) {
            Id = row.id;
          }
          return `<a href="/ItemReceipt/Edit/${Id}" m-1">Xem | Sửa</a>`;
        },
      },

      // 2. Thanh toán
      {
        data: null,
        width: "80px",
        orderable: false,
        render: function (data, type, row) {
          var Id = "";
          if (type === "display" && data !== null) {
            Id = row.id;
          }
          return `<a href="/BillPayment/Payment/${Id}" m-1">Thanh toán</a>`;
        },
      },

      // 3. STT
      {
        data: null,
        name: "STT",
        width: "50px",
        autoWidth: true,
        orderable: false,
        searchable: false,
        render: function (data, type, row, meta) {
          return meta.row + meta.settings._iDisplayStart + 1;
        },
      },

      // 4. Tên hàng (tên kho)
      { data: "inventoryName", name: "inventoryName", autoWidth: true },

      // 5. Mã vận đơn
      { data: "shipCode", name: "shipCode", autoWidth: true },

      // 6. Mã đơn hàng
      { data: "orderCode", name: "orderCode", autoWidth: true },

      // 7. Tên đơn hàng
      { data: "orderName", name: "orderName", autoWidth: true },

      // 8. Trạng thái
      {
        data: "status",
        name: "status",
        autoWidth: true,
        orderable: false,
        render: function (data, type, row) {
          if (data === 0) {
            return '<span class="badge badge-success">Hoàn thành</span>';
          } else if (data === 1) {
            return '<span class="badge badge-danger">Không thành công</span>';
          } else {
            return '<span class="badge badge-secondary">Unknown</span>';
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
