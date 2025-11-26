$(document).ready(function () {
  var table = $("#customerDatatable").DataTable({
    processing: true,
    serverSide: true,
    filter: true,
    ajax: {
      url: "/api/ProductApi",
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

      // 1. Action Xem | Sửa
      {
        width: "80px",
        orderable: false,
        render: function (data, type, row) {
          var Id = "";
          if (type === "display" && data !== null) {
            Id = row.id;
          }
          return `<a href="/Product/Edit/${Id}" m-1">Xem | Sửa</a>`;
        },
      },

      // 2. STT
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

      // 3. Mã hàng hoá
      { data: "productCode", name: "productCode", autoWidth: true },

      // 4. Tên hàng hoá
      { data: "productName", name: "productName", autoWidth: true },

      // 5. Đơn vị  (tên field JSON là "units")
      { data: "units", name: "Units", autoWidth: true },

      // 6. Nhà cung cấp
      { data: "venderName", name: "venderName", autoWidth: true },

      // 7. Cột icon xoá
      {
        data: null,
        orderable: false,
        searchable: false,
        width: "40px",
        render: function (data, type, row) {
          return `
            <a href="javascript:void(0);"
               class="text-danger btn-delete"
               data-id="${row.id}"
               title="Xoá">
                <i class="fa fa-trash"></i>
            </a>`;
        },
      },
    ],
    lengthMenu: [
      [5, 10, 20, 50, 100],
      [5, 10, 20, 50, 100],
    ],
    pageLength: 5,
  });

  // Xử lý xoá
  $("#customerDatatable").on("click", ".btn-delete", function () {
    var id = $(this).data("id");

    if (!confirm("Bạn có chắc muốn xoá mặt hàng này?")) {
      return;
    }

    $.ajax({
      url: "/Product/DeleteAjax",
      type: "POST",
      data: { id: id },
      success: function (res) {
        if (res.success) {
          table.ajax.reload(null, false);
          if (typeof showToast === "function") {
            showToast(res.message || "Xoá thành công");
          } else {
            alert(res.message || "Xoá thành công");
          }
        } else {
          if (typeof showToast === "function") {
            showToast(res.message || "Xoá thất bại", "error");
          } else {
            alert(res.message || "Xoá thất bại");
          }
        }
      },
      error: function () {
        if (typeof showToast === "function") {
          showToast("Có lỗi xảy ra khi xoá.", "error");
        } else {
          alert("Có lỗi xảy ra khi xoá.");
        }
      },
    });
  });
});
