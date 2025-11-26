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
      { data: "id", name: "Id", autoWidth: true },
      {
        data: null,
        render: function (data, type, row, meta) {
          return meta.row + meta.settings._iDisplayStart + 1;
        },
      },
      { data: "productName", name: "productName", autoWidth: true },
      { data: "quantity", name: "quantity", autoWidth: true },
      //   {
      //     data: "taxAmount",
      //     name: "taxAmount",
      //     autoWidth: true,
      //     render: function (data, type, row) {
      //       // Định dạng Tiền Việt Nam Đồng
      //       return new Intl.NumberFormat("vi-VN", {
      //         style: "currency",
      //         currency: "VND",
      //       }).format(data);
      //     },
      //   },
      {
        data: "price",
        name: "price",
        autoWidth: true,
        render: function (data, type, row) {
          // Định dạng Tiền Việt Nam Đồng
          return new Intl.NumberFormat("vi-VN", {
            style: "currency",
            currency: "VND",
          }).format(data);
        },
      },
      //   {
      //     data: "discountAmount",
      //     name: "discountAmount",
      //     autoWidth: true,
      //     render: function (data, type, row) {
      //       // Định dạng Tiền Việt Nam Đồng
      //       return new Intl.NumberFormat("vi-VN", {
      //         style: "currency",
      //         currency: "VND",
      //       }).format(data);
      //     },
      //   },
      {
        data: "totalAmount",
        name: "totalAmount",
        autoWidth: true,
        render: function (data, type, row) {
          // Định dạng Tiền Việt Nam Đồng
          return new Intl.NumberFormat("vi-VN", {
            style: "currency",
            currency: "VND",
          }).format(data);
        },
      },
    ],
    footerCallback: function (row, data, start, end, display) {
      var api = this.api();

      // Chuyển giá trị thành số thực và tính tổng
      var total = api
        .column(5, { page: "current" }) // Cột thứ 7 (0-based index) là `totalAmount`
        .data()
        .reduce(function (a, b) {
          // Xử lý giá trị null hoặc không phải số
          return parseFloat(a) + parseFloat(b || 0);
        }, 0);

      // Hiển thị tổng tiền trong footer
      $(api.column(5).footer()).html(
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
