$(document).ready(function () {
  $("#customerDatatable").DataTable({
    processing: true,
    serverSide: true,
    filter: true,
    ajax: {
      url: "/api/ReturnAuthorizationApi",
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
      // 0: ID (ẩn)
      { data: "id", name: "ID", autoWidth: true },

      // 1: Link xem / sửa
      {
        targets: 1,
        width: "80px",
        orderable: false,
        render: function (data, type, row) {
          var Id = "";
          if (type === "display" && row && row.id) {
            Id = row.id;
          }
          return `<a href="/ReturnAuthorization/Edit/${Id}" m-1">Xem | Sửa</a>`;
        },
      },

      // 2: STT
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

      // 3: Mã trả hàng
      {
        data: "returnAuthorizationCode",
        name: "ReturnAuthorizationCode",
        autoWidth: true,
      },

      // 4: Mã đặt hàng
      { data: "nameOrder", name: "nameOrder", autoWidth: true },

      // 5: Tên đơn hàng
      { data: "orderName", name: "orderName", autoWidth: true },

      // 6: Ngày trả
      {
        data: "dateShip",
        name: "dateShip",
        autoWidth: true,
        render: function (data, type, row) {
          if (!data) return "";
          // Chuẩn hóa sang định dạng ngày tiếng Việt
          var d = new Date(data);
          if (isNaN(d)) return data;
          return d.toLocaleDateString("vi-VN");
        },
      },

      // 7: Lý do
      {
        data: "description",
        name: "description",
        autoWidth: true,
      },

      // 8: Tiền trả lại
      {
        data: "amountReturn",
        name: "amountReturn",
        autoWidth: true,
        render: function (data, type, row) {
          if (data === null || data === undefined) return "";
          return new Intl.NumberFormat("vi-VN", {
            style: "currency",
            currency: "VND",
          }).format(data);
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
      location.reload();
    },
    error: function (xhr, status, error) {
      console.log(xhr.responseText);
    },
  });

  $.ajax({
    url: "/api/CategoryApi/SendMes",
    type: "POST",
    data: {},
    success: function (response) {
      alert(response);
    },
    error: function (xhr, status, error) {},
  });
}

function EditEmp(id) {}
