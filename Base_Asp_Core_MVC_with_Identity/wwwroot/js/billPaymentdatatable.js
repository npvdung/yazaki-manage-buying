$(document).ready(function () {
  $("#customerDatatable").DataTable({
    processing: true,
    serverSide: true,
    filter: true,
    ajax: {
      url: "/api/BillPaymentApi",
      type: "GET",
      datatype: "json",
      dataSrc: "data",
    },
    columnDefs: [
      {
        targets: [0], // ẩn cột Id
        visible: false,
        searchable: false,
      },
    ],
    columns: [
      // 0. Id (ẩn)
      { data: "id", name: "Id", autoWidth: true },

      // 1. Nút IN hóa đơn
      {
        data: null,
        width: "80px",
        orderable: false,
        render: function (data, type, row) {
          var id = row.id;
          return `
            <a href="/BillPayment/Print/${id}"
               class="btn btn-sm btn-outline-secondary">
                <i class="fa fa-print"></i> In
            </a>`;
        },
      },

      // 2. Action Xem | Sửa
      {
        data: null,
        width: "80px",
        orderable: false,
        render: function (data, type, row) {
          var id = row.id;
          return `<a href="/BillPayment/Edit/${id}" m-1">Xem | Sửa</a>`;
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

      // 4. Mã hóa đơn
      { data: "billPaymentCode", name: "billPaymentCode", autoWidth: true },

      // 5. Ngày thanh toán
      {
        data: "billPaymentDate",
        name: "billPaymentDate",
        autoWidth: true,
        render: function (data, type, row) {
          if (!data) return "";
          var date = new Date(data);
          return date.toLocaleDateString("vi-VN");
        },
      },

      // 6. Tổng tiền
      {
        data: "totalAmount",
        name: "totalAmount",
        autoWidth: true,
        render: function (data, type, row) {
          if (data == null) return "";
          return new Intl.NumberFormat("vi-VN", {
            style: "currency",
            currency: "VND",
          }).format(data);
        },
      },

      // 7. Mã đơn hàng (tương ứng OrderCode trong API)
      {
        data: "orderCode",
        name: "orderCode",
        autoWidth: true,
      },

      // 8. Tên đơn hàng (OrderName trong API)
      {
        data: "orderName",
        name: "orderName",
        autoWidth: true,
      },

      // 9. Trạng thái (StatusText trong API) hiển thị dạng badge
      {
        data: "statusText",
        name: "statusText",
        autoWidth: true,
        orderable: false,
        render: function (data, type, row) {
          if (!data) return "";

          // tùy vào text mà gắn màu khác nhau
          if (data === "Đã nhận") {
            return (
              '<span class="badge badge-success">' + "Đã thanh toán" + "</span>"
            );
          } else if (data === "Thất bại") {
            return '<span class="badge badge-danger">' + data + "</span>";
          } else {
            return '<span class="badge badge-secondary">' + data + "</span>";
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
    },
    success: function (response) {
      alert(response);
    },
    error: function (xhr, status, error) {
      // Xử lý lỗi nếu cần
    },
  });
}

function EditEmp(id) {}
