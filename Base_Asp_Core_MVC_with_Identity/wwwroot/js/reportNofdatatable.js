$(document).ready(function () {
  $("#customerDatatable").DataTable({
    processing: true,
    serverSide: true,
    ajax: {
      url: "/api/ReportApi",
      type: "GET",
      datatype: "json",
      dataSrc: "data",
    },
    columnDefs: [
      {
        targets: [0], // ẩn cột ID
        visible: false,
        searchable: false,
      },
    ],
    columns: [
      { data: "id", name: "id", autoWidth: true }, // hidden

      {
        data: null,
        name: "stt",
        render: function (data, type, row, meta) {
          return meta.row + meta.settings._iDisplayStart + 1;
        },
      },
      { data: "orderCode", name: "orderCode", autoWidth: true },

      { data: "productName", name: "productName", autoWidth: true },
      { data: "quantity", name: "quantity", autoWidth: true },

      {
        data: "price",
        name: "price",
        autoWidth: true,
        render: function (data, type, row) {
          return new Intl.NumberFormat("vi-VN", {
            style: "currency",
            currency: "VND",
          }).format(data || 0);
        },
      },

      {
        data: "totalAmount",
        name: "totalAmount",
        autoWidth: true,
        render: function (data, type, row) {
          return new Intl.NumberFormat("vi-VN", {
            style: "currency",
            currency: "VND",
          }).format(data || 0);
        },
      },

      // Ngày đặt hàng = DateShip
      {
        data: "orderDate",
        name: "orderDate",
        autoWidth: true,
        render: function (data, type, row) {
          if (!data) return "";
          var date = new Date(data);
          if (isNaN(date.getTime())) return "";
          return date.toLocaleDateString("vi-VN");
        },
      },

      // Ngày thanh toán
      {
        data: "paymentDate",
        name: "paymentDate",
        autoWidth: true,
        render: function (data, type, row) {
          if (!data) return "";
          var date = new Date(data);
          if (isNaN(date.getTime())) return "";
          return date.toLocaleDateString("vi-VN");
        },
      },

      // Nhà cung cấp
      { data: "vendorName", name: "vendorName", autoWidth: true },

      // Tiền tệ
      { data: "currencyName", name: "currencyName", autoWidth: true },
    ],

    footerCallback: function (row, data, start, end, display) {
      var api = this.api();

      // cột 5 (index bắt đầu từ 0) là "totalAmount"
      var total = api
        .column(6, { page: "current" })
        .data()
        .reduce(function (a, b) {
          var x = parseFloat(a) || 0;
          var y = parseFloat(b) || 0;
          return x + y;
        }, 0);

      $(api.column(6).footer()).html(
        "Tổng: " + total.toLocaleString("vi-VN") + " VND"
      );
    },

    lengthMenu: [
      [5, 10, 20, 50, 100],
      [5, 10, 20, 50, 100],
    ],
    pageLength: 5,
  });
});
