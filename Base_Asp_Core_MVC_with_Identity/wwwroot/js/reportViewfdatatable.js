$(document).ready(function () {
  $("#customerDatatable").DataTable({
    processing: true,
    serverSide: true,
    ajax: {
      url: "/api/ReportViewApi",
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
      { data: "id", name: "id", autoWidth: true },
      {
        data: null,
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
          }).format(data);
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
          }).format(data);
        },
      },
      {
        // NGÀY TRẢ LẠI HÀNG
        data: "returnDate",
        name: "returnDate",
        autoWidth: true,
        render: function (data, type, row) {
          if (!data) return "";
          return new Date(data).toLocaleDateString("vi-VN");
        },
      },
      {
        // LÍ DO TRẢ HÀNG
        data: "reasonReturn",
        name: "reasonReturn",
        autoWidth: true,
      },
      {
        data: "vendorName",
        name: "vendorName",
        autoWidth: true,
      },
      {
        data: "currencyName",
        name: "currencyName",
        autoWidth: true,
      },
    ],
    footerCallback: function (row, data, start, end, display) {
      var api = this.api();

      // tính tổng cột totalAmount (index 5)
      var total = api
        .column(6, { page: "current" })
        .data()
        .reduce(function (a, b) {
          return parseFloat(a) + parseFloat(b || 0);
        }, 0);

      $(api.column(6).footer()).html(
        "Tổng: " + total.toLocaleString() + " VND"
      );
    },
    lengthMenu: [
      [5, 10, 20, 50, 100],
      [5, 10, 20, 50, 100],
    ],
    pageLength: 5,
  });
});
