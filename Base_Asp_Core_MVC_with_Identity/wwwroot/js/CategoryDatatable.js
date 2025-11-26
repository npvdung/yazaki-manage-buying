$(document).ready(function () {
  console.log("CategoryDatatable loaded");

  var table = $("#customerDatatable").DataTable({
    processing: true,
    serverSide: true,
    filter: true,
    ajax: {
      url: "/api/CategoryApi",
      type: "GET",
      datatype: "json",
      dataSrc: "data",
    },

    // sort mặc định theo cột "Mã vật tư" (index 2)
    order: [[2, "asc"]],

    columns: [
      // 0: Action (View/Edit)
      {
        data: null,
        name: "", // không dùng để sort
        width: "80px",
        orderable: false,
        render: function (data, type, row) {
          var Id = "";
          if (type === "display" && data !== null) {
            Id = row.id;
          }
          return `<a href="/Category/Edit/${Id}" m-1">Xem | Sửa</a>`;
        },
      },

      // 1: STT
      {
        data: null,
        name: "", // không dùng để sort
        width: "50px",
        autoWidth: true,
        orderable: false,
        searchable: false,
        render: function (data, type, row, meta) {
          return meta.row + meta.settings._iDisplayStart + 1;
        },
      },

      // 2: Mã vật tư  -> tên property trong C# là CategoryCode
      { data: "categoryCode", name: "CategoryCode", autoWidth: true },

      // 3: Tên loại vật tư -> CategoryName
      { data: "categoryName", name: "CategoryName", autoWidth: true },

      // 4: Mô tả -> Description
      {
        data: "description",
        name: "Description",
        autoWidth: true,
        orderable: false,
      },

      // 5: Icon xoá
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

  // Bắt sự kiện click icon xoá
  $("#customerDatatable").on("click", ".btn-delete", function () {
    var id = $(this).data("id");

    if (!confirm("Bạn có chắc muốn xoá loại vật tư này?")) {
      return;
    }

    $.ajax({
      url: "/Category/DeleteAjax",
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
