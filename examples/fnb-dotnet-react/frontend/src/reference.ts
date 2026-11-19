// Các định nghĩa form cụ thể tương ứng model SDK .NET.
import type { Field, Operation } from "./types";

export const schemas: Record<string, Field[]> = {
  "AttributeValue": [
    {
      "name": "id",
      "label": "ID bản ghi",
      "type": "string",
      "optional": false,
      "description": "Tạo giá trị thuộc tính. Tạo giá trị thuộc tính. ID thuộc tính. Giá trị chọn. ID thuộc tính.",
      "kind": "string"
    },
    {
      "name": "value",
      "label": "Giá trị",
      "type": "string",
      "optional": true,
      "description": "Giá trị chọn.",
      "kind": "string"
    }
  ],
  "ComboGroupRequest": [
    {
      "name": "title",
      "label": "Tên chương trình",
      "type": "string",
      "optional": false,
      "description": "Tên nhóm.",
      "kind": "string"
    },
    {
      "name": "requiredQuantity",
      "label": "Số lượng phải chọn",
      "type": "int",
      "optional": false,
      "description": "Số món khách cần chọn trong nhóm.",
      "kind": "number"
    },
    {
      "name": "products",
      "label": "Sản phẩm",
      "type": "List<ComboProductRequest>",
      "optional": false,
      "description": "Sản phẩm được chọn trong nhóm.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "ComboProductRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "ComboProductRequest"
      }
    }
  ],
  "ComboProductRequest": [
    {
      "name": "productId",
      "label": "Sản phẩm",
      "type": "Guid",
      "optional": false,
      "description": "ID sản phẩm.",
      "kind": "guid"
    },
    {
      "name": "unitId",
      "label": "Đơn vị tính",
      "type": "Guid",
      "optional": true,
      "description": "ID đơn vị tính; null là đơn vị cơ bản.",
      "kind": "guid"
    },
    {
      "name": "unitName",
      "label": "unitName",
      "type": "string",
      "optional": true,
      "description": "Tên đơn vị tính.",
      "kind": "string"
    },
    {
      "name": "quantity",
      "label": "Số lượng",
      "type": "int",
      "optional": false,
      "description": "Số lượng. Mặc định 1.",
      "kind": "number",
      "default": 1
    },
    {
      "name": "totalPrice",
      "label": "totalPrice",
      "type": "decimal",
      "optional": false,
      "description": "Thành tiền của sản phẩm trong combo (chưa gồm VAT).",
      "kind": "number"
    }
  ],
  "ComboRequest": [
    {
      "name": "title",
      "label": "Tên chương trình",
      "type": "string",
      "optional": false,
      "description": "Tên combo (bắt buộc).",
      "kind": "string"
    },
    {
      "name": "description",
      "label": "description",
      "type": "string",
      "optional": true,
      "description": "Mô tả.",
      "kind": "string"
    },
    {
      "name": "isActive",
      "label": "Đang hoạt động",
      "type": "bool",
      "optional": false,
      "description": "Đang hoạt động.",
      "kind": "boolean"
    },
    {
      "name": "comboType",
      "label": "Loại combo",
      "type": "ComboType",
      "optional": false,
      "description": "Loại combo: cố định dùng , tự chọn dùng .",
      "kind": "enum",
      "options": [
        {
          "label": "Fixed",
          "value": 0
        },
        {
          "label": "Mix",
          "value": 1
        }
      ]
    },
    {
      "name": "pricingMode",
      "label": "Cách tính giá",
      "type": "ComboPricingMode",
      "optional": false,
      "description": "Cách tính giá.",
      "kind": "enum",
      "options": [
        {
          "label": "Common",
          "value": 0
        },
        {
          "label": "SumOfProducts",
          "value": 1
        }
      ]
    },
    {
      "name": "price",
      "label": "Giá bán",
      "type": "decimal",
      "optional": false,
      "description": "Giá chung của combo, chưa gồm VAT (khi là ).",
      "kind": "number"
    },
    {
      "name": "vat",
      "label": "vat",
      "type": "int",
      "optional": false,
      "description": "Thuế suất VAT (%) của giá chung.",
      "kind": "number"
    },
    {
      "name": "taxIndustryCategoryId",
      "label": "taxIndustryCategoryId",
      "type": "string",
      "optional": true,
      "description": "Mã ngành nghề tính thuế.",
      "kind": "string"
    },
    {
      "name": "usageLimit",
      "label": "Số lượt sử dụng",
      "type": "int",
      "optional": true,
      "description": "Số lần sử dụng (combo dịch vụ nhiều lần). Mặc định 1.",
      "kind": "number",
      "default": 1
    },
    {
      "name": "expiredInDays",
      "label": "expiredInDays",
      "type": "int",
      "optional": true,
      "description": "Thời hạn sử dụng (ngày) tính từ ngày mua (combo dịch vụ).",
      "kind": "number"
    },
    {
      "name": "image",
      "label": "Ảnh",
      "type": "ImageUpload",
      "optional": true,
      "description": "Ảnh; null giữ ảnh hiện tại.",
      "kind": "image"
    },
    {
      "name": "fixedProducts",
      "label": "fixedProducts",
      "type": "List<ComboProductRequest>",
      "optional": false,
      "description": "Sản phẩm của combo cố định.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "ComboProductRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "ComboProductRequest"
      }
    },
    {
      "name": "groups",
      "label": "Nhóm lựa chọn",
      "type": "List<ComboGroupRequest>",
      "optional": false,
      "description": "Nhóm lựa chọn của combo tự chọn.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "ComboGroupRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "ComboGroupRequest"
      }
    }
  ],
  "CustomerRequest": [
    {
      "name": "customerName",
      "label": "Tên khách hàng",
      "type": "string",
      "optional": false,
      "description": "Tên khách hàng (bắt buộc).",
      "kind": "string"
    },
    {
      "name": "customerPhone",
      "label": "Số điện thoại",
      "type": "string",
      "optional": false,
      "description": "Số điện thoại (bắt buộc, tối đa 20 ký tự, không trùng khách khác của cửa hàng).",
      "kind": "string"
    },
    {
      "name": "customerAddress",
      "label": "Địa chỉ",
      "type": "string",
      "optional": true,
      "description": "Địa chỉ.",
      "kind": "string"
    },
    {
      "name": "customerCccd",
      "label": "CCCD",
      "type": "string",
      "optional": true,
      "description": "Số CCCD.",
      "kind": "string"
    },
    {
      "name": "gender",
      "label": "Giới tính",
      "type": "bool",
      "optional": true,
      "description": "Giới tính: true nam, false nữ, null chưa xác định.",
      "kind": "boolean"
    },
    {
      "name": "birthDate",
      "label": "Ngày sinh",
      "type": "DateTime",
      "optional": true,
      "description": "Ngày sinh.",
      "kind": "datetime"
    }
  ],
  "FlashSaleProductRequest": [
    {
      "name": "productId",
      "label": "Sản phẩm",
      "type": "Guid",
      "optional": false,
      "description": "ID sản phẩm.",
      "kind": "guid"
    },
    {
      "name": "unitId",
      "label": "Đơn vị tính",
      "type": "Guid",
      "optional": true,
      "description": "ID đơn vị tính; null là đơn vị cơ bản.",
      "kind": "guid"
    },
    {
      "name": "unitName",
      "label": "unitName",
      "type": "string",
      "optional": true,
      "description": "Tên đơn vị tính.",
      "kind": "string"
    },
    {
      "name": "price",
      "label": "Giá bán",
      "type": "decimal",
      "optional": false,
      "description": "Giá flash sale.",
      "kind": "number"
    }
  ],
  "FlashSaleRequest": [
    {
      "name": "title",
      "label": "Tên chương trình",
      "type": "string",
      "optional": false,
      "description": "Tên flash sale (bắt buộc).",
      "kind": "string"
    },
    {
      "name": "description",
      "label": "description",
      "type": "string",
      "optional": true,
      "description": "Mô tả.",
      "kind": "string"
    },
    {
      "name": "isActive",
      "label": "Đang hoạt động",
      "type": "bool",
      "optional": false,
      "description": "Đang hoạt động.",
      "kind": "boolean"
    },
    {
      "name": "startDate",
      "label": "Bắt đầu",
      "type": "DateTime",
      "optional": false,
      "description": "Thời điểm bắt đầu (bắt buộc).",
      "kind": "datetime"
    },
    {
      "name": "endDate",
      "label": "Kết thúc",
      "type": "DateTime",
      "optional": true,
      "description": "Thời điểm kết thúc; null là không giới hạn.",
      "kind": "datetime"
    },
    {
      "name": "image",
      "label": "Ảnh",
      "type": "ImageUpload",
      "optional": true,
      "description": "Ảnh; null giữ ảnh hiện tại.",
      "kind": "image"
    },
    {
      "name": "branchIds",
      "label": "Chi nhánh áp dụng",
      "type": "List<Guid>",
      "optional": false,
      "description": "Chi nhánh áp dụng.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    },
    {
      "name": "saleChannelIds",
      "label": "Kênh bán áp dụng",
      "type": "List<string>",
      "optional": false,
      "description": "Kênh bán áp dụng (đã cấu hình cho cửa hàng).",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "string",
        "optional": false,
        "description": "",
        "kind": "string"
      }
    },
    {
      "name": "products",
      "label": "Sản phẩm",
      "type": "List<FlashSaleProductRequest>",
      "optional": false,
      "description": "Sản phẩm và giá flash sale.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "FlashSaleProductRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "FlashSaleProductRequest"
      }
    }
  ],
  "FnbMoveChildItem": [
    {
      "name": "id",
      "label": "ID bản ghi",
      "type": "Guid",
      "optional": false,
      "description": "lineId của dòng món kèm.",
      "kind": "guid"
    }
  ],
  "FnbMoveItem": [
    {
      "name": "id",
      "label": "ID bản ghi",
      "type": "Guid",
      "optional": false,
      "description": "lineId của dòng trong đơn nguồn.",
      "kind": "guid"
    },
    {
      "name": "quantity",
      "label": "Số lượng",
      "type": "decimal",
      "optional": false,
      "description": "Số lượng chuyển.",
      "kind": "number"
    },
    {
      "name": "childs",
      "label": "Món kèm chuyển cùng",
      "type": "List<FnbMoveChildItem>",
      "optional": false,
      "description": "Dòng món kèm đi cùng món chính.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "FnbMoveChildItem",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "FnbMoveChildItem"
      }
    }
  ],
  "FnbMoveItemsRequest": [
    {
      "name": "targetOrderId",
      "label": "Đơn đích",
      "type": "Guid",
      "optional": true,
      "description": "Đơn đích đang mở cùng chi nhánh; null để tạo đơn mới.",
      "kind": "guid"
    },
    {
      "name": "saleChannelId",
      "label": "Kênh bán",
      "type": "string",
      "optional": false,
      "description": "Kênh bán của đơn mới. Mặc định TaiCho .",
      "kind": "string",
      "default": "TaiCho"
    },
    {
      "name": "targetTableId",
      "label": "Bàn đích",
      "type": "Guid",
      "optional": true,
      "description": "Bàn của đơn mới (khi không có ), phải còn trống.",
      "kind": "guid"
    },
    {
      "name": "products",
      "label": "Sản phẩm",
      "type": "List<FnbMoveItem>",
      "optional": false,
      "description": "Dòng món cần chuyển.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "FnbMoveItem",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "FnbMoveItem"
      }
    },
    {
      "name": "combos",
      "label": "Combo",
      "type": "List<FnbMoveItem>",
      "optional": false,
      "description": "Dòng combo cần chuyển.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "FnbMoveItem",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "FnbMoveItem"
      }
    }
  ],
  "FnbOrderAddOnRequest": [
    {
      "name": "productId",
      "label": "Sản phẩm",
      "type": "Guid",
      "optional": false,
      "description": "ID sản phẩm món kèm.",
      "kind": "guid"
    },
    {
      "name": "quantity",
      "label": "Số lượng",
      "type": "decimal",
      "optional": false,
      "description": "Số lượng. Mặc định 1.",
      "kind": "number",
      "default": 1
    },
    {
      "name": "unitId",
      "label": "Đơn vị tính",
      "type": "Guid",
      "optional": true,
      "description": "ID đơn vị tính; null là đơn vị cơ bản.",
      "kind": "guid"
    }
  ],
  "FnbOrderComboGroupRequest": [
    {
      "name": "groupId",
      "label": "Nhóm combo",
      "type": "Guid",
      "optional": false,
      "description": "ID nhóm của combo.",
      "kind": "guid"
    },
    {
      "name": "products",
      "label": "Sản phẩm",
      "type": "List<FnbOrderComboProductRequest>",
      "optional": false,
      "description": "Món được chọn.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "FnbOrderComboProductRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "FnbOrderComboProductRequest"
      }
    }
  ],
  "FnbOrderComboProductRequest": [
    {
      "name": "productId",
      "label": "Sản phẩm",
      "type": "Guid",
      "optional": false,
      "description": "ID sản phẩm.",
      "kind": "guid"
    },
    {
      "name": "quantity",
      "label": "Số lượng",
      "type": "decimal",
      "optional": false,
      "description": "Số lượng. Mặc định 1.",
      "kind": "number",
      "default": 1
    },
    {
      "name": "unitId",
      "label": "Đơn vị tính",
      "type": "Guid",
      "optional": true,
      "description": "ID đơn vị tính; null là đơn vị cơ bản.",
      "kind": "guid"
    }
  ],
  "FnbOrderComboRequest": [
    {
      "name": "comboId",
      "label": "Combo",
      "type": "Guid",
      "optional": false,
      "description": "ID combo đang bán.",
      "kind": "guid"
    },
    {
      "name": "quantity",
      "label": "Số lượng",
      "type": "decimal",
      "optional": false,
      "description": "Số lượng. Mặc định 1.",
      "kind": "number",
      "default": 1
    },
    {
      "name": "note",
      "label": "Ghi chú",
      "type": "string",
      "optional": true,
      "description": "Ghi chú.",
      "kind": "string"
    },
    {
      "name": "groups",
      "label": "Nhóm lựa chọn",
      "type": "List<FnbOrderComboGroupRequest>",
      "optional": false,
      "description": "Lựa chọn cho từng nhóm của combo tự chọn.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "FnbOrderComboGroupRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "FnbOrderComboGroupRequest"
      }
    }
  ],
  "FnbOrderCreateRequest": [
    {
      "name": "requestId",
      "label": "Request ID chống trùng",
      "type": "Guid",
      "optional": true,
      "description": "Khóa chống tạo trùng do đối tác sinh. Gọi lại với cùng giá trị trả về đơn đã tạo thay vì tạo đơn mới. Nên luôn đặt giá trị này (ví dụ ) và lưu lại trước khi gọi API.",
      "kind": "guid"
    },
    {
      "name": "saleChannelId",
      "label": "Kênh bán",
      "type": "string",
      "optional": false,
      "description": "Kênh bán đang hoạt động. Mặc định TaiCho .",
      "kind": "string",
      "default": "TaiCho"
    },
    {
      "name": "tableId",
      "label": "Bàn",
      "type": "Guid",
      "optional": true,
      "description": "Bàn thuộc chi nhánh, đang hoạt động và chưa có đơn.",
      "kind": "guid"
    },
    {
      "name": "note",
      "label": "Ghi chú",
      "type": "string",
      "optional": true,
      "description": "Ghi chú (tối đa 1000 ký tự).",
      "kind": "string"
    },
    {
      "name": "customerId",
      "label": "Khách hàng",
      "type": "Guid",
      "optional": true,
      "description": "Khách hàng của cửa hàng.",
      "kind": "guid"
    },
    {
      "name": "items",
      "label": "Món",
      "type": "List<FnbOrderItemRequest>",
      "optional": false,
      "description": "Món gọi.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "FnbOrderItemRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "FnbOrderItemRequest"
      }
    },
    {
      "name": "combos",
      "label": "Combo",
      "type": "List<FnbOrderComboRequest>",
      "optional": false,
      "description": "Combo gọi.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "FnbOrderComboRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "FnbOrderComboRequest"
      }
    },
    {
      "name": "attributes",
      "label": "Thuộc tính",
      "type": "List<AttributeValue>",
      "optional": false,
      "description": "Thuộc tính đơn hàng.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "AttributeValue",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "AttributeValue"
      }
    }
  ],
  "FnbOrderItemRequest": [
    {
      "name": "productId",
      "label": "Sản phẩm",
      "type": "Guid",
      "optional": false,
      "description": "ID sản phẩm (món đang bán).",
      "kind": "guid"
    },
    {
      "name": "quantity",
      "label": "Số lượng",
      "type": "decimal",
      "optional": false,
      "description": "Số lượng. Mặc định 1.",
      "kind": "number",
      "default": 1
    },
    {
      "name": "note",
      "label": "Ghi chú",
      "type": "string",
      "optional": true,
      "description": "Ghi chú món.",
      "kind": "string"
    },
    {
      "name": "unitId",
      "label": "Đơn vị tính",
      "type": "Guid",
      "optional": true,
      "description": "ID đơn vị tính; null là đơn vị cơ bản.",
      "kind": "guid"
    },
    {
      "name": "flashSaleProductId",
      "label": "flashSaleProductId",
      "type": "Guid",
      "optional": true,
      "description": "ID sản phẩm flash sale lấy từ menu ( FnbMenuProduct.FlashSaleProductId ).",
      "kind": "guid"
    },
    {
      "name": "attributes",
      "label": "Thuộc tính",
      "type": "List<AttributeValue>",
      "optional": false,
      "description": "Thuộc tính món (thuộc nhóm sản phẩm của món).",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "AttributeValue",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "AttributeValue"
      }
    },
    {
      "name": "addOns",
      "label": "Món kèm",
      "type": "List<FnbOrderAddOnRequest>",
      "optional": false,
      "description": "Món kèm (thuộc cấu hình món kèm của món chính).",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "FnbOrderAddOnRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "FnbOrderAddOnRequest"
      }
    }
  ],
  "FnbOrdersCancelBody": [
    {
      "name": "cancelReason",
      "label": "Lý do hủy",
      "type": "string",
      "optional": false,
      "description": "",
      "kind": "string"
    }
  ],
  "FnbOrdersCancelComboBody": [
    {
      "name": "reason",
      "label": "Lý do",
      "type": "string",
      "optional": false,
      "description": "",
      "kind": "string"
    }
  ],
  "FnbOrdersCancelProductBody": [
    {
      "name": "quantity",
      "label": "Số lượng",
      "type": "decimal",
      "optional": false,
      "description": "",
      "kind": "number"
    },
    {
      "name": "reason",
      "label": "Lý do",
      "type": "string",
      "optional": false,
      "description": "",
      "kind": "string"
    }
  ],
  "FnbOrdersChangeTableBody": [
    {
      "name": "tableId",
      "label": "Bàn",
      "type": "Guid",
      "optional": true,
      "description": "",
      "kind": "guid"
    },
    {
      "name": "saleChannelId",
      "label": "Kênh bán",
      "type": "string",
      "optional": true,
      "description": "",
      "kind": "string"
    }
  ],
  "FnbOrdersMergeBody": [
    {
      "name": "sourceOrderId",
      "label": "Đơn nguồn",
      "type": "Guid",
      "optional": false,
      "description": "",
      "kind": "guid"
    },
    {
      "name": "targetOrderId",
      "label": "Đơn đích",
      "type": "Guid",
      "optional": false,
      "description": "",
      "kind": "guid"
    }
  ],
  "FnbOrdersSetCustomerBody": [
    {
      "name": "customerId",
      "label": "Khách hàng",
      "type": "Guid",
      "optional": true,
      "description": "",
      "kind": "guid"
    }
  ],
  "FnbOrdersUpdateNoteBody": [
    {
      "name": "note",
      "label": "Ghi chú",
      "type": "string",
      "optional": true,
      "description": "",
      "kind": "string"
    }
  ],
  "FnbOrdersUpdateProductNoteBody": [
    {
      "name": "note",
      "label": "Ghi chú",
      "type": "string",
      "optional": true,
      "description": "",
      "kind": "string"
    }
  ],
  "FnbOrdersUpdateProductPriceBody": [
    {
      "name": "price",
      "label": "Giá bán",
      "type": "decimal",
      "optional": false,
      "description": "",
      "kind": "number"
    },
    {
      "name": "reason",
      "label": "Lý do",
      "type": "string",
      "optional": true,
      "description": "",
      "kind": "string"
    }
  ],
  "FnbOrdersUpdateProductQuantityBody": [
    {
      "name": "quantity",
      "label": "Số lượng",
      "type": "decimal",
      "optional": false,
      "description": "",
      "kind": "number"
    },
    {
      "name": "reason",
      "label": "Lý do",
      "type": "string",
      "optional": true,
      "description": "",
      "kind": "string"
    }
  ],
  "FnbOrdersUpdateProductUnitBody": [
    {
      "name": "unitId",
      "label": "Đơn vị tính",
      "type": "Guid",
      "optional": true,
      "description": "",
      "kind": "guid"
    }
  ],
  "FnbPaymentQrRequest": [
    {
      "name": "paymentProvider",
      "label": "Nhà cung cấp QR",
      "type": "string",
      "optional": false,
      "description": "Nhà cung cấp QR ( PaymentOption.Provider có SupportsQr ), bắt buộc.",
      "kind": "string"
    },
    {
      "name": "customerPayment",
      "label": "Tiền khách trả",
      "type": "decimal",
      "optional": false,
      "description": "Số tiền của QR, không lớn hơn tổng tiền đơn.",
      "kind": "number"
    },
    {
      "name": "pixel",
      "label": "Kích thước QR",
      "type": "int",
      "optional": false,
      "description": "Kích thước điểm ảnh của ảnh QR. Mặc định 1.",
      "kind": "number",
      "default": 1
    },
    {
      "name": "discount",
      "label": "Mức giảm",
      "type": "OrderDiscountRequest",
      "optional": true,
      "description": "Chiết khấu; null để server tự áp chương trình đang khớp.",
      "kind": "object",
      "ref": "OrderDiscountRequest"
    },
    {
      "name": "surcharge",
      "label": "Phụ thu",
      "type": "OrderSurchargeRequest",
      "optional": true,
      "description": "Phụ thu; null để server tự áp chương trình đang khớp.",
      "kind": "object",
      "ref": "OrderSurchargeRequest"
    },
    {
      "name": "voucher",
      "label": "Voucher",
      "type": "OrderVoucherRequest",
      "optional": true,
      "description": "Voucher.",
      "kind": "object",
      "ref": "OrderVoucherRequest"
    },
    {
      "name": "membership",
      "label": "Đổi điểm thành viên",
      "type": "OrderMembershipRequest",
      "optional": true,
      "description": "Điểm thành viên.",
      "kind": "object",
      "ref": "OrderMembershipRequest"
    }
  ],
  "FnbPaymentRequest": [
    {
      "name": "discount",
      "label": "Mức giảm",
      "type": "OrderDiscountRequest",
      "optional": true,
      "description": "Chiết khấu; null để server tự áp chương trình đang khớp.",
      "kind": "object",
      "ref": "OrderDiscountRequest"
    },
    {
      "name": "surcharge",
      "label": "Phụ thu",
      "type": "OrderSurchargeRequest",
      "optional": true,
      "description": "Phụ thu; null để server tự áp chương trình đang khớp.",
      "kind": "object",
      "ref": "OrderSurchargeRequest"
    },
    {
      "name": "voucher",
      "label": "Voucher",
      "type": "OrderVoucherRequest",
      "optional": true,
      "description": "Voucher.",
      "kind": "object",
      "ref": "OrderVoucherRequest"
    },
    {
      "name": "membership",
      "label": "Đổi điểm thành viên",
      "type": "OrderMembershipRequest",
      "optional": true,
      "description": "Điểm thành viên.",
      "kind": "object",
      "ref": "OrderMembershipRequest"
    },
    {
      "name": "invoiceInfo",
      "label": "invoiceInfo",
      "type": "OrderInvoiceInfo",
      "optional": true,
      "description": "Thông tin xuất hóa đơn điện tử.",
      "kind": "object",
      "ref": "OrderInvoiceInfo"
    },
    {
      "name": "paymentMethod",
      "label": "Phương thức thanh toán",
      "type": "PaymentMethod",
      "optional": false,
      "description": "Phương thức thanh toán. Mặc định tiền mặt.",
      "kind": "enum",
      "options": [
        {
          "label": "Cash",
          "value": 1
        },
        {
          "label": "Bank",
          "value": 2
        }
      ],
      "default": 1
    },
    {
      "name": "paymentProvider",
      "label": "Nhà cung cấp QR",
      "type": "string",
      "optional": true,
      "description": "Nhà cung cấp thanh toán ( PaymentOption.Provider ) khi thanh toán qua ngân hàng/QR.",
      "kind": "string"
    },
    {
      "name": "customerPayment",
      "label": "Tiền khách trả",
      "type": "decimal",
      "optional": false,
      "description": "Số tiền khách trả.",
      "kind": "number"
    }
  ],
  "OrderAdjustmentProgramRequest": [
    {
      "name": "title",
      "label": "Tên chương trình",
      "type": "string",
      "optional": false,
      "description": "Tên chương trình (bắt buộc, tối đa 200 ký tự).",
      "kind": "string"
    },
    {
      "name": "type",
      "label": "Loại",
      "type": "OrderAdjustmentType",
      "optional": false,
      "description": "Loại: chiết khấu hoặc phụ thu.",
      "kind": "enum",
      "options": [
        {
          "label": "Discount",
          "value": 1
        },
        {
          "label": "Surcharge",
          "value": 2
        }
      ],
      "default": 1
    },
    {
      "name": "valueType",
      "label": "Kiểu giá trị",
      "type": "DiscountType",
      "optional": false,
      "description": "Kiểu giá trị: theo % hoặc số tiền.",
      "kind": "enum",
      "options": [
        {
          "label": "None",
          "value": 0
        },
        {
          "label": "Percent",
          "value": 1
        },
        {
          "label": "Amount",
          "value": 2
        }
      ],
      "default": 1
    },
    {
      "name": "value",
      "label": "Giá trị",
      "type": "decimal",
      "optional": false,
      "description": "Giá trị.",
      "kind": "number"
    },
    {
      "name": "isApplyBeforeVAT",
      "label": "Áp dụng trước VAT",
      "type": "bool",
      "optional": false,
      "description": "Áp dụng trên giá trị trước VAT.",
      "kind": "boolean"
    },
    {
      "name": "priority",
      "label": "Ưu tiên",
      "type": "int",
      "optional": false,
      "description": "Thứ tự ưu tiên.",
      "kind": "number"
    },
    {
      "name": "isActive",
      "label": "Đang hoạt động",
      "type": "bool",
      "optional": false,
      "description": "Đang hoạt động. Mặc định true .",
      "kind": "boolean",
      "default": true
    },
    {
      "name": "fromDate",
      "label": "Từ ngày",
      "type": "DateTime",
      "optional": true,
      "description": "Từ ngày.",
      "kind": "datetime"
    },
    {
      "name": "toDate",
      "label": "Đến ngày",
      "type": "DateTime",
      "optional": true,
      "description": "Đến ngày.",
      "kind": "datetime"
    },
    {
      "name": "version",
      "label": "version",
      "type": "int",
      "optional": true,
      "description": "Phiên bản đã đọc từ chi tiết, gửi lại khi sửa để chống ghi đè (sai trả 422). null thì không kiểm tra.",
      "kind": "number"
    },
    {
      "name": "branchIds",
      "label": "Chi nhánh áp dụng",
      "type": "List<Guid>",
      "optional": false,
      "description": "Chi nhánh áp dụng; rỗng là tất cả.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    },
    {
      "name": "saleChannelIds",
      "label": "Kênh bán áp dụng",
      "type": "List<string>",
      "optional": false,
      "description": "Kênh bán áp dụng; rỗng là tất cả.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "string",
        "optional": false,
        "description": "",
        "kind": "string"
      }
    },
    {
      "name": "daysOfWeek",
      "label": "Ngày trong tuần",
      "type": "List<DayOfWeek>",
      "optional": false,
      "description": "Ngày trong tuần áp dụng; rỗng là tất cả.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "DayOfWeek",
        "optional": false,
        "description": "",
        "kind": "enum",
        "options": [
          {
            "label": "Chủ nhật",
            "value": 0
          },
          {
            "label": "Thứ hai",
            "value": 1
          },
          {
            "label": "Thứ ba",
            "value": 2
          },
          {
            "label": "Thứ tư",
            "value": 3
          },
          {
            "label": "Thứ năm",
            "value": 4
          },
          {
            "label": "Thứ sáu",
            "value": 5
          },
          {
            "label": "Thứ bảy",
            "value": 6
          }
        ]
      }
    },
    {
      "name": "timeSlots",
      "label": "Khung giờ",
      "type": "List<TimeSlot>",
      "optional": false,
      "description": "Khung giờ áp dụng; rỗng là cả ngày.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "TimeSlot",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "TimeSlot"
      }
    },
    {
      "name": "membershipRankIds",
      "label": "Hạng thành viên",
      "type": "List<Guid>",
      "optional": false,
      "description": "Hạng thành viên áp dụng; rỗng là tất cả.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    }
  ],
  "OrderDiscountRequest": [
    {
      "name": "discount",
      "label": "Mức giảm",
      "type": "decimal",
      "optional": false,
      "description": "Không áp chiết khấu (gửi {\"discount\": 0} ). Giá trị chiết khấu (theo % tối đa 100, hoặc số tiền).",
      "kind": "number"
    },
    {
      "name": "discountType",
      "label": "Loại giảm",
      "type": "DiscountType",
      "optional": false,
      "description": "Kiểu chiết khấu.",
      "kind": "enum",
      "options": [
        {
          "label": "None",
          "value": 0
        },
        {
          "label": "Percent",
          "value": 1
        },
        {
          "label": "Amount",
          "value": 2
        }
      ],
      "default": 1
    },
    {
      "name": "isApplyBeforeVAT",
      "label": "Áp dụng trước VAT",
      "type": "bool",
      "optional": false,
      "description": "Tính trên giá trị trước VAT.",
      "kind": "boolean"
    }
  ],
  "OrderInvoiceInfo": [
    {
      "name": "buyerCCCD",
      "label": "buyerCCCD",
      "type": "string",
      "optional": true,
      "description": "Số CCCD người mua.",
      "kind": "string"
    },
    {
      "name": "buyerHoChieu",
      "label": "buyerHoChieu",
      "type": "string",
      "optional": true,
      "description": "Số hộ chiếu người mua.",
      "kind": "string"
    },
    {
      "name": "buyerName",
      "label": "buyerName",
      "type": "string",
      "optional": true,
      "description": "Tên người mua.",
      "kind": "string"
    },
    {
      "name": "buyerAddress",
      "label": "buyerAddress",
      "type": "string",
      "optional": true,
      "description": "Địa chỉ người mua.",
      "kind": "string"
    },
    {
      "name": "buyerPhone",
      "label": "buyerPhone",
      "type": "string",
      "optional": true,
      "description": "Số điện thoại người mua.",
      "kind": "string"
    },
    {
      "name": "companyTaxCode",
      "label": "companyTaxCode",
      "type": "string",
      "optional": true,
      "description": "Mã số thuế đơn vị mua.",
      "kind": "string"
    },
    {
      "name": "companyName",
      "label": "companyName",
      "type": "string",
      "optional": true,
      "description": "Tên đơn vị mua.",
      "kind": "string"
    },
    {
      "name": "companyEmail",
      "label": "companyEmail",
      "type": "string",
      "optional": true,
      "description": "Email nhận hóa đơn của đơn vị mua.",
      "kind": "string"
    },
    {
      "name": "companyAddress",
      "label": "companyAddress",
      "type": "string",
      "optional": true,
      "description": "Địa chỉ đơn vị mua.",
      "kind": "string"
    },
    {
      "name": "companyPhone",
      "label": "companyPhone",
      "type": "string",
      "optional": true,
      "description": "Số điện thoại đơn vị mua.",
      "kind": "string"
    },
    {
      "name": "isBusinessInvoice",
      "label": "isBusinessInvoice",
      "type": "bool",
      "optional": false,
      "description": "Xuất hóa đơn cho doanh nghiệp.",
      "kind": "boolean"
    },
    {
      "name": "canEdit",
      "label": "canEdit",
      "type": "bool",
      "optional": false,
      "description": "Cho phép sửa thông tin hóa đơn.",
      "kind": "boolean"
    },
    {
      "name": "lookupUrl",
      "label": "lookupUrl",
      "type": "string",
      "optional": true,
      "description": "Đường dẫn tra cứu hóa đơn.",
      "kind": "string"
    }
  ],
  "OrderMembershipRequest": [
    {
      "name": "rankId",
      "label": "rankId",
      "type": "Guid",
      "optional": true,
      "description": "Hạng thành viên của khách.",
      "kind": "guid"
    },
    {
      "name": "pointApplied",
      "label": "Điểm sử dụng",
      "type": "int",
      "optional": false,
      "description": "Số điểm sử dụng.",
      "kind": "number"
    },
    {
      "name": "burnRate",
      "label": "Tỷ lệ quy đổi điểm",
      "type": "decimal",
      "optional": false,
      "description": "Tỷ lệ quy đổi điểm thành tiền của hạng ( MembershipRank.BurnRate ).",
      "kind": "number"
    },
    {
      "name": "pointBurnTaxBasis",
      "label": "Cơ sở VAT đổi điểm",
      "type": "PointBurnTaxBasis",
      "optional": false,
      "description": "Trừ trên giá trị trước hay sau VAT ( MembershipRank.PointBurnTaxBasis ).",
      "kind": "enum",
      "options": [
        {
          "label": "BeforeVat",
          "value": 1
        },
        {
          "label": "AfterVat",
          "value": 2
        }
      ],
      "default": 2
    }
  ],
  "OrderSurchargeRequest": [
    {
      "name": "surcharge",
      "label": "Phụ thu",
      "type": "decimal",
      "optional": false,
      "description": "Không áp phụ thu (gửi {\"surcharge\": 0} ). Giá trị phụ thu.",
      "kind": "number"
    },
    {
      "name": "surchargeType",
      "label": "Loại phụ thu",
      "type": "DiscountType",
      "optional": false,
      "description": "Kiểu phụ thu.",
      "kind": "enum",
      "options": [
        {
          "label": "None",
          "value": 0
        },
        {
          "label": "Percent",
          "value": 1
        },
        {
          "label": "Amount",
          "value": 2
        }
      ],
      "default": 1
    },
    {
      "name": "isApplyBeforeVAT",
      "label": "Áp dụng trước VAT",
      "type": "bool",
      "optional": false,
      "description": "Tính trên giá trị trước VAT.",
      "kind": "boolean"
    },
    {
      "name": "reason",
      "label": "Lý do",
      "type": "string",
      "optional": true,
      "description": "Lý do phụ thu.",
      "kind": "string"
    }
  ],
  "OrderVoucherRequest": [
    {
      "name": "voucherCode",
      "label": "Mã voucher",
      "type": "string",
      "optional": false,
      "description": "Tạo voucher theo mã. Tạo voucher theo mã. Mã voucher. Mã voucher.",
      "kind": "string"
    }
  ],
  "PricingPoliciesGetProductAddOnsBody": [
    {
      "name": "productIds",
      "label": "Sản phẩm áp dụng",
      "type": "IEnumerable<Guid>",
      "optional": false,
      "description": "",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    }
  ],
  "PricingPolicyProduct": [
    {
      "name": "productId",
      "label": "Sản phẩm",
      "type": "Guid",
      "optional": false,
      "description": "ID sản phẩm.",
      "kind": "guid"
    },
    {
      "name": "unitId",
      "label": "Đơn vị tính",
      "type": "Guid",
      "optional": true,
      "description": "ID đơn vị tính; null hoặc GUID rỗng là đơn vị cơ bản.",
      "kind": "guid"
    },
    {
      "name": "price",
      "label": "Giá bán",
      "type": "decimal",
      "optional": false,
      "description": "Giá (chưa gồm VAT nếu có trường VAT đi kèm).",
      "kind": "number"
    },
    {
      "name": "addOns",
      "label": "Món kèm",
      "type": "List<PricingPolicyProductAddOn>",
      "optional": true,
      "description": "Danh sách món kèm.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "PricingPolicyProductAddOn",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "PricingPolicyProductAddOn"
      }
    }
  ],
  "PricingPolicyProductAddOn": [
    {
      "name": "addOnProductId",
      "label": "addOnProductId",
      "type": "Guid",
      "optional": false,
      "description": "ID sản phẩm món kèm.",
      "kind": "guid"
    },
    {
      "name": "addOnUnitId",
      "label": "addOnUnitId",
      "type": "Guid",
      "optional": true,
      "description": "ID đơn vị tính của món kèm; null là đơn vị cơ bản.",
      "kind": "guid"
    },
    {
      "name": "price",
      "label": "Giá bán",
      "type": "decimal",
      "optional": false,
      "description": "Giá (chưa gồm VAT nếu có trường VAT đi kèm).",
      "kind": "number"
    }
  ],
  "PricingPolicyRequest": [
    {
      "name": "title",
      "label": "Tên chương trình",
      "type": "string",
      "optional": false,
      "description": "Tên chính sách (bắt buộc).",
      "kind": "string"
    },
    {
      "name": "priority",
      "label": "Ưu tiên",
      "type": "int",
      "optional": false,
      "description": "Số nhỏ hơn được ưu tiên khi nhiều chính sách cùng khớp.",
      "kind": "number"
    },
    {
      "name": "isActive",
      "label": "Đang hoạt động",
      "type": "bool",
      "optional": false,
      "description": "Đang hoạt động. Mặc định true .",
      "kind": "boolean",
      "default": true
    },
    {
      "name": "fromDate",
      "label": "Từ ngày",
      "type": "DateTime",
      "optional": true,
      "description": "Từ ngày (tính theo ngày).",
      "kind": "datetime"
    },
    {
      "name": "toDate",
      "label": "Đến ngày",
      "type": "DateTime",
      "optional": true,
      "description": "Đến ngày (tính theo ngày), không nhỏ hơn .",
      "kind": "datetime"
    },
    {
      "name": "branchIds",
      "label": "Chi nhánh áp dụng",
      "type": "List<Guid>",
      "optional": false,
      "description": "Chi nhánh áp dụng; rỗng là tất cả.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    },
    {
      "name": "saleChannelIds",
      "label": "Kênh bán áp dụng",
      "type": "List<string>",
      "optional": false,
      "description": "Kênh bán áp dụng; rỗng là tất cả.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "string",
        "optional": false,
        "description": "",
        "kind": "string"
      }
    },
    {
      "name": "daysOfWeek",
      "label": "Ngày trong tuần",
      "type": "List<DayOfWeek>",
      "optional": false,
      "description": "Ngày trong tuần áp dụng; rỗng là tất cả.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "DayOfWeek",
        "optional": false,
        "description": "",
        "kind": "enum",
        "options": [
          {
            "label": "Chủ nhật",
            "value": 0
          },
          {
            "label": "Thứ hai",
            "value": 1
          },
          {
            "label": "Thứ ba",
            "value": 2
          },
          {
            "label": "Thứ tư",
            "value": 3
          },
          {
            "label": "Thứ năm",
            "value": 4
          },
          {
            "label": "Thứ sáu",
            "value": 5
          },
          {
            "label": "Thứ bảy",
            "value": 6
          }
        ]
      }
    },
    {
      "name": "products",
      "label": "Sản phẩm",
      "type": "List<PricingPolicyProduct>",
      "optional": false,
      "description": "Giá sản phẩm; sản phẩm và đơn vị tính không trùng nhau.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "PricingPolicyProduct",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "PricingPolicyProduct"
      }
    },
    {
      "name": "timeSlots",
      "label": "Khung giờ",
      "type": "List<TimeSlot>",
      "optional": false,
      "description": "Khung giờ áp dụng, không chồng lấn; rỗng là cả ngày.",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "TimeSlot",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "TimeSlot"
      }
    }
  ],
  "ProductCreateRequest": [
    {
      "name": "productCode",
      "label": "Mã sản phẩm",
      "type": "string",
      "optional": false,
      "description": "Mã sản phẩm trên POS (bắt buộc, tối đa 300 ký tự, phân biệt hoa thường).",
      "kind": "string"
    },
    {
      "name": "name",
      "label": "Tên",
      "type": "string",
      "optional": false,
      "description": "Tên sản phẩm (bắt buộc, tối đa 500 ký tự).",
      "kind": "string"
    },
    {
      "name": "price",
      "label": "Giá bán",
      "type": "decimal",
      "optional": true,
      "description": "Giá bán đã gồm VAT, mặc định 0.",
      "kind": "number"
    },
    {
      "name": "unit",
      "label": "Đơn vị",
      "type": "string",
      "optional": true,
      "description": "Đơn vị tính (tối đa 30 ký tự), mặc định Cái .",
      "kind": "string"
    }
  ],
  "ProductUpdateRequest": [
    {
      "name": "name",
      "label": "Tên",
      "type": "string",
      "optional": false,
      "description": "Tên sản phẩm (bắt buộc, tối đa 500 ký tự).",
      "kind": "string"
    },
    {
      "name": "price",
      "label": "Giá bán",
      "type": "decimal",
      "optional": true,
      "description": "Giá bán đã gồm VAT; null giữ giá cũ.",
      "kind": "number"
    },
    {
      "name": "unit",
      "label": "Đơn vị",
      "type": "string",
      "optional": true,
      "description": "Đơn vị tính; null giữ giá trị cũ.",
      "kind": "string"
    }
  ],
  "QrOrderCodeAttributeRequest": [
    {
      "name": "attributeId",
      "label": "attributeId",
      "type": "string",
      "optional": false,
      "description": "ID thuộc tính đơn hàng (từ QrOrderCodes.GetOrderAttributesAsync ).",
      "kind": "string"
    },
    {
      "name": "isRequired",
      "label": "Bắt buộc",
      "type": "bool",
      "optional": false,
      "description": "Bắt buộc nhập.",
      "kind": "boolean"
    },
    {
      "name": "priority",
      "label": "Ưu tiên",
      "type": "int",
      "optional": false,
      "description": "Thứ tự hiển thị.",
      "kind": "number"
    }
  ],
  "QrOrderCodeRequest": [
    {
      "name": "branchId",
      "label": "Chi nhánh",
      "type": "Guid",
      "optional": false,
      "description": "Chi nhánh FnB đang hoạt động (bắt buộc).",
      "kind": "guid"
    },
    {
      "name": "name",
      "label": "Tên",
      "type": "string",
      "optional": true,
      "description": "Tên QR, tối đa 200 ký tự.",
      "kind": "string"
    },
    {
      "name": "saleChannelId",
      "label": "Kênh bán",
      "type": "string",
      "optional": false,
      "description": "Kênh bán (bắt buộc), lấy từ QrOrderCodes.GetSaleChannelsAsync .",
      "kind": "string"
    },
    {
      "name": "tableId",
      "label": "Bàn",
      "type": "Guid",
      "optional": true,
      "description": "Bàn; bắt buộc khi kênh TaiCho , kênh khác bị bỏ qua.",
      "kind": "guid"
    },
    {
      "name": "hours",
      "label": "hours",
      "type": "int",
      "optional": false,
      "description": "Số giờ hiệu lực tính từ lúc lưu.",
      "kind": "number"
    },
    {
      "name": "minutes",
      "label": "minutes",
      "type": "int",
      "optional": false,
      "description": "Số phút hiệu lực (0–59).",
      "kind": "number"
    },
    {
      "name": "noExpiry",
      "label": "noExpiry",
      "type": "bool",
      "optional": false,
      "description": "true : không giới hạn thời gian, bỏ qua / .",
      "kind": "boolean"
    },
    {
      "name": "requireCustomerInfo",
      "label": "requireCustomerInfo",
      "type": "bool",
      "optional": false,
      "description": "Bắt buộc khách nhập tên và số điện thoại.",
      "kind": "boolean"
    },
    {
      "name": "isSingleUse",
      "label": "isSingleUse",
      "type": "bool",
      "optional": false,
      "description": "QR chỉ tạo được 1 đơn.",
      "kind": "boolean"
    },
    {
      "name": "attributes",
      "label": "Thuộc tính",
      "type": "List<QrOrderCodeAttributeRequest>",
      "optional": false,
      "description": "Thuộc tính đơn khách cần nhập, không trùng attributeId .",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "QrOrderCodeAttributeRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "QrOrderCodeAttributeRequest"
      }
    }
  ],
  "QrOrderCodesSetActiveBody": [
    {
      "name": "isActive",
      "label": "Đang hoạt động",
      "type": "bool",
      "optional": false,
      "description": "",
      "kind": "boolean"
    }
  ],
  "SaleChannelCreateRequest": [
    {
      "name": "saleChannelId",
      "label": "Kênh bán",
      "type": "string",
      "optional": false,
      "description": "ID kênh trong danh mục hệ thống (bắt buộc), ví dụ GiaoHang .",
      "kind": "string"
    },
    {
      "name": "isActive",
      "label": "Đang hoạt động",
      "type": "bool",
      "optional": false,
      "description": "Đang hoạt động. Mặc định true .",
      "kind": "boolean",
      "default": true
    },
    {
      "name": "priority",
      "label": "Ưu tiên",
      "type": "int",
      "optional": false,
      "description": "Thứ tự ưu tiên.",
      "kind": "number"
    },
    {
      "name": "isECommerce",
      "label": "isECommerce",
      "type": "SaleChannelECommerceType",
      "optional": false,
      "description": "Loại kênh bán.",
      "kind": "enum",
      "options": [
        {
          "label": "Offline",
          "value": 0
        },
        {
          "label": "ECommerceWithPayment",
          "value": 1
        },
        {
          "label": "ECommerceWithoutPayment",
          "value": 2
        }
      ],
      "default": 0
    }
  ],
  "TimeSlot": [
    {
      "name": "startTime",
      "label": "Bắt đầu",
      "type": "TimeOnly",
      "optional": false,
      "description": "Giờ bắt đầu.",
      "kind": "time"
    },
    {
      "name": "endTime",
      "label": "Kết thúc",
      "type": "TimeOnly",
      "optional": false,
      "description": "Giờ kết thúc.",
      "kind": "time"
    }
  ],
  "VoucherRequest": [
    {
      "name": "title",
      "label": "Tên chương trình",
      "type": "string",
      "optional": false,
      "description": "Tên voucher (bắt buộc).",
      "kind": "string"
    },
    {
      "name": "brief",
      "label": "Mô tả ngắn",
      "type": "string",
      "optional": true,
      "description": "Mô tả ngắn.",
      "kind": "string"
    },
    {
      "name": "content",
      "label": "Nội dung",
      "type": "string",
      "optional": true,
      "description": "Nội dung chi tiết.",
      "kind": "string"
    },
    {
      "name": "startDate",
      "label": "Bắt đầu",
      "type": "DateTime",
      "optional": false,
      "description": "Ngày bắt đầu.",
      "kind": "datetime"
    },
    {
      "name": "endDate",
      "label": "Kết thúc",
      "type": "DateTime",
      "optional": true,
      "description": "Ngày kết thúc, không nhỏ hơn ngày bắt đầu; khi tạo không nhỏ hơn hiện tại.",
      "kind": "datetime"
    },
    {
      "name": "isActive",
      "label": "Đang hoạt động",
      "type": "bool",
      "optional": false,
      "description": "Trạng thái hoạt động (bị bỏ qua khi tạo).",
      "kind": "boolean"
    },
    {
      "name": "discountType",
      "label": "Loại giảm",
      "type": "DiscountType",
      "optional": false,
      "description": "Kiểu giảm: theo % hoặc số tiền.",
      "kind": "enum",
      "options": [
        {
          "label": "None",
          "value": 0
        },
        {
          "label": "Percent",
          "value": 1
        },
        {
          "label": "Amount",
          "value": 2
        }
      ],
      "default": 1
    },
    {
      "name": "discount",
      "label": "Mức giảm",
      "type": "decimal",
      "optional": false,
      "description": "Giá trị giảm.",
      "kind": "number"
    },
    {
      "name": "max",
      "label": "Giảm tối đa",
      "type": "decimal",
      "optional": false,
      "description": "Số tiền giảm tối đa; 0 là không giới hạn.",
      "kind": "number"
    },
    {
      "name": "min",
      "label": "Giá trị đơn tối thiểu",
      "type": "decimal",
      "optional": false,
      "description": "Giá trị đơn tối thiểu; 0 là không giới hạn.",
      "kind": "number"
    },
    {
      "name": "usageLimit",
      "label": "Số lượt sử dụng",
      "type": "int",
      "optional": false,
      "description": "Số lần dùng mỗi mã; 0 là không giới hạn.",
      "kind": "number"
    },
    {
      "name": "voucherNumber",
      "label": "Số lượng mã",
      "type": "int",
      "optional": false,
      "description": "Số lượng mã (lớn hơn 0); hệ thống sinh mã 6 ký tự viết hoa.",
      "kind": "number",
      "default": 1
    },
    {
      "name": "voucherCode",
      "label": "Mã voucher",
      "type": "string",
      "optional": true,
      "description": "Mã nhập tay, chỉ dùng khi bằng 1; không trùng mã hiện có.",
      "kind": "string"
    },
    {
      "name": "isApplyBeforeVAT",
      "label": "Áp dụng trước VAT",
      "type": "bool",
      "optional": false,
      "description": "Áp dụng trên giá trị trước VAT. Mặc định true .",
      "kind": "boolean",
      "default": true
    },
    {
      "name": "image",
      "label": "Ảnh",
      "type": "ImageUpload",
      "optional": true,
      "description": "Ảnh; null giữ ảnh hiện tại.",
      "kind": "image"
    }
  ],
  "VouchersSetActiveBody": [
    {
      "name": "isActive",
      "label": "Đang hoạt động",
      "type": "bool",
      "optional": false,
      "description": "",
      "kind": "boolean"
    }
  ],
  "SaleChannelUpdateRequest": [
    {
      "name": "saleChannelId",
      "label": "Kênh bán",
      "type": "string",
      "optional": false,
      "description": "ID kênh bán lấy từ tham số đường dẫn.",
      "kind": "string"
    },
    {
      "name": "isActive",
      "label": "Đang hoạt động",
      "type": "bool",
      "optional": false,
      "description": "Kênh TaiCho không được tắt.",
      "kind": "boolean"
    },
    {
      "name": "priority",
      "label": "Ưu tiên",
      "type": "int",
      "optional": false,
      "description": "Thứ tự hiển thị.",
      "kind": "number"
    },
    {
      "name": "isECommerce",
      "label": "Loại kênh bán",
      "type": "SaleChannelECommerceType",
      "optional": false,
      "description": "Loại kênh bán.",
      "kind": "enum",
      "options": [
        {
          "label": "Offline",
          "value": 0
        },
        {
          "label": "ECommerceWithPayment",
          "value": 1
        },
        {
          "label": "ECommerceWithoutPayment",
          "value": 2
        }
      ],
      "default": 0
    }
  ]
};

export const operations: Operation[] = [
  {
    "id": "Lookups.GetBranchesAsync",
    "group": "Lookups",
    "groupLabel": "Danh mục tra cứu",
    "label": "Chi nhánh",
    "method": "GET",
    "path": "lookups/branches",
    "pathFields": [],
    "queryFields": [],
    "body": null
  },
  {
    "id": "Lookups.GetMembershipRanksAsync",
    "group": "Lookups",
    "groupLabel": "Danh mục tra cứu",
    "label": "Hạng thành viên",
    "method": "GET",
    "path": "lookups/membership-ranks",
    "pathFields": [],
    "queryFields": [],
    "body": null
  },
  {
    "id": "Products.ListAsync",
    "group": "Products",
    "groupLabel": "Sản phẩm",
    "label": "Danh sách",
    "method": "GET",
    "path": "products",
    "pathFields": [],
    "queryFields": [
      {
        "name": "pageNumber",
        "label": "Trang",
        "type": "int",
        "optional": false,
        "description": "Trang cần lấy, bắt đầu từ 1. Mặc định 1.",
        "kind": "number",
        "default": 1
      },
      {
        "name": "pageSize",
        "label": "Số dòng / trang",
        "type": "int",
        "optional": false,
        "description": "Số phần tử mỗi trang, nên từ 1 đến 100. Mặc định 15.",
        "kind": "number",
        "default": 15
      },
      {
        "name": "keyword",
        "label": "Từ khóa",
        "type": "string",
        "optional": true,
        "description": "Tìm theo mã hoặc tên sản phẩm (không phân biệt hoa thường).",
        "kind": "string"
      }
    ],
    "body": null
  },
  {
    "id": "Products.GetAsync",
    "group": "Products",
    "groupLabel": "Sản phẩm",
    "label": "Chi tiết",
    "method": "GET",
    "path": "products/{productId}",
    "pathFields": [
      {
        "name": "productId",
        "label": "Sản phẩm",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "Products.GetUnitsAsync",
    "group": "Products",
    "groupLabel": "Sản phẩm",
    "label": "Đơn vị tính",
    "method": "GET",
    "path": "products/{productId}/units",
    "pathFields": [
      {
        "name": "productId",
        "label": "Sản phẩm",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "Products.CreateAsync",
    "group": "Products",
    "groupLabel": "Sản phẩm",
    "label": "Tạo mới",
    "method": "POST",
    "path": "products",
    "pathFields": [],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "ProductCreateRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "ProductCreateRequest"
    }
  },
  {
    "id": "Products.UpdateAsync",
    "group": "Products",
    "groupLabel": "Sản phẩm",
    "label": "Cập nhật",
    "method": "PUT",
    "path": "products/{productId}",
    "pathFields": [
      {
        "name": "productId",
        "label": "Sản phẩm",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "ProductUpdateRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "ProductUpdateRequest"
    }
  },
  {
    "id": "Products.DeleteAsync",
    "group": "Products",
    "groupLabel": "Sản phẩm",
    "label": "Xóa",
    "method": "POST",
    "path": "products/{productId}/delete",
    "pathFields": [
      {
        "name": "productId",
        "label": "Sản phẩm",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "QrOrderCodes.GetBranchesAsync",
    "group": "QrOrderCodes",
    "groupLabel": "QR đặt món",
    "label": "Chi nhánh",
    "method": "GET",
    "path": "qr-order-codes/branches",
    "pathFields": [],
    "queryFields": [],
    "body": null
  },
  {
    "id": "QrOrderCodes.GetSaleChannelsAsync",
    "group": "QrOrderCodes",
    "groupLabel": "QR đặt món",
    "label": "Kênh bán cho QR",
    "method": "GET",
    "path": "qr-order-codes/sale-channels",
    "pathFields": [],
    "queryFields": [],
    "body": null
  },
  {
    "id": "QrOrderCodes.GetTablesAsync",
    "group": "QrOrderCodes",
    "groupLabel": "QR đặt món",
    "label": "Bàn",
    "method": "GET",
    "path": "qr-order-codes/tables",
    "pathFields": [],
    "queryFields": [
      {
        "name": "branchId",
        "label": "Chi nhánh",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "body": null
  },
  {
    "id": "QrOrderCodes.GetOrderAttributesAsync",
    "group": "QrOrderCodes",
    "groupLabel": "QR đặt món",
    "label": "Thuộc tính đơn",
    "method": "GET",
    "path": "qr-order-codes/order-attributes",
    "pathFields": [],
    "queryFields": [],
    "body": null
  },
  {
    "id": "QrOrderCodes.ListAsync",
    "group": "QrOrderCodes",
    "groupLabel": "QR đặt món",
    "label": "Danh sách",
    "method": "GET",
    "path": "qr-order-codes",
    "pathFields": [],
    "queryFields": [
      {
        "name": "pageNumber",
        "label": "Trang",
        "type": "int",
        "optional": false,
        "description": "Trang cần lấy, bắt đầu từ 1. Mặc định 1.",
        "kind": "number",
        "default": 1
      },
      {
        "name": "pageSize",
        "label": "Số dòng / trang",
        "type": "int",
        "optional": false,
        "description": "Số phần tử mỗi trang, nên từ 1 đến 100. Mặc định 15.",
        "kind": "number",
        "default": 15
      },
      {
        "name": "branchId",
        "label": "Chi nhánh",
        "type": "Guid",
        "optional": true,
        "description": "Chi nhánh.",
        "kind": "guid"
      },
      {
        "name": "saleChannelId",
        "label": "Kênh bán",
        "type": "string",
        "optional": true,
        "description": "Kênh bán.",
        "kind": "string"
      },
      {
        "name": "status",
        "label": "Trạng thái",
        "type": "int",
        "optional": true,
        "description": "Trạng thái: null tất cả, 1 đang hiệu lực, 2 đã tắt, 3 hết hạn.",
        "kind": "number"
      },
      {
        "name": "keyword",
        "label": "Từ khóa",
        "type": "string",
        "optional": true,
        "description": "Tìm theo tên QR hoặc tên bàn.",
        "kind": "string"
      }
    ],
    "body": null
  },
  {
    "id": "QrOrderCodes.GetAsync",
    "group": "QrOrderCodes",
    "groupLabel": "QR đặt món",
    "label": "Chi tiết",
    "method": "GET",
    "path": "qr-order-codes/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "QrOrderCodes.CreateAsync",
    "group": "QrOrderCodes",
    "groupLabel": "QR đặt món",
    "label": "Tạo mới",
    "method": "POST",
    "path": "qr-order-codes",
    "pathFields": [],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "QrOrderCodeRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "QrOrderCodeRequest"
    }
  },
  {
    "id": "QrOrderCodes.UpdateAsync",
    "group": "QrOrderCodes",
    "groupLabel": "QR đặt món",
    "label": "Cập nhật",
    "method": "PUT",
    "path": "qr-order-codes/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "QrOrderCodeRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "QrOrderCodeRequest"
    }
  },
  {
    "id": "QrOrderCodes.SetActiveAsync",
    "group": "QrOrderCodes",
    "groupLabel": "QR đặt món",
    "label": "Bật / tắt",
    "method": "PUT",
    "path": "qr-order-codes/{id}/active",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "QrOrderCodesSetActiveBody",
      "type": "QrOrderCodesSetActiveBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "QrOrderCodes.GetQrImageAsync",
    "group": "QrOrderCodes",
    "groupLabel": "QR đặt món",
    "label": "Ảnh QR đặt món",
    "method": "GET",
    "path": "qr-order-codes/{id}/qr",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "QrOrderCodes.DeleteAsync",
    "group": "QrOrderCodes",
    "groupLabel": "QR đặt món",
    "label": "Xóa",
    "method": "POST",
    "path": "qr-order-codes/{id}/delete",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "Customers.ListAsync",
    "group": "Customers",
    "groupLabel": "Khách hàng",
    "label": "Danh sách",
    "method": "GET",
    "path": "customers",
    "pathFields": [],
    "queryFields": [
      {
        "name": "pageNumber",
        "label": "Trang",
        "type": "int",
        "optional": false,
        "description": "Trang cần lấy, bắt đầu từ 1. Mặc định 1.",
        "kind": "number",
        "default": 1
      },
      {
        "name": "pageSize",
        "label": "Số dòng / trang",
        "type": "int",
        "optional": false,
        "description": "Số phần tử mỗi trang, nên từ 1 đến 100. Mặc định 15.",
        "kind": "number",
        "default": 15
      },
      {
        "name": "key",
        "label": "Từ khóa",
        "type": "string",
        "optional": true,
        "description": "Từ khóa (tên, số điện thoại).",
        "kind": "string"
      },
      {
        "name": "fromDate",
        "label": "Từ ngày",
        "type": "DateTime",
        "optional": true,
        "description": "Từ ngày.",
        "kind": "datetime"
      },
      {
        "name": "toDate",
        "label": "Đến ngày",
        "type": "DateTime",
        "optional": true,
        "description": "Đến ngày.",
        "kind": "datetime"
      },
      {
        "name": "rankId",
        "label": "rankId",
        "type": "Guid",
        "optional": true,
        "description": "Hạng thành viên.",
        "kind": "guid"
      }
    ],
    "body": null
  },
  {
    "id": "Customers.GetAsync",
    "group": "Customers",
    "groupLabel": "Khách hàng",
    "label": "Chi tiết",
    "method": "GET",
    "path": "customers/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "Customers.CreateAsync",
    "group": "Customers",
    "groupLabel": "Khách hàng",
    "label": "Tạo mới",
    "method": "POST",
    "path": "customers",
    "pathFields": [],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "CustomerRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "CustomerRequest"
    }
  },
  {
    "id": "Customers.UpdateAsync",
    "group": "Customers",
    "groupLabel": "Khách hàng",
    "label": "Cập nhật",
    "method": "PUT",
    "path": "customers/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "CustomerRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "CustomerRequest"
    }
  },
  {
    "id": "Customers.DeleteAsync",
    "group": "Customers",
    "groupLabel": "Khách hàng",
    "label": "Xóa",
    "method": "POST",
    "path": "customers/{id}/delete",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "PricingPolicies.ListAsync",
    "group": "PricingPolicies",
    "groupLabel": "Chính sách giá",
    "label": "Danh sách",
    "method": "GET",
    "path": "pricing-policies",
    "pathFields": [],
    "queryFields": [
      {
        "name": "pageNumber",
        "label": "Trang",
        "type": "int",
        "optional": false,
        "description": "Trang cần lấy, bắt đầu từ 1. Mặc định 1.",
        "kind": "number",
        "default": 1
      },
      {
        "name": "pageSize",
        "label": "Số dòng / trang",
        "type": "int",
        "optional": false,
        "description": "Số phần tử mỗi trang, nên từ 1 đến 100. Mặc định 15.",
        "kind": "number",
        "default": 15
      },
      {
        "name": "key",
        "label": "Từ khóa",
        "type": "string",
        "optional": true,
        "description": "Từ khóa theo tên.",
        "kind": "string"
      },
      {
        "name": "isActive",
        "label": "Đang hoạt động",
        "type": "bool",
        "optional": true,
        "description": "Trạng thái hoạt động.",
        "kind": "boolean"
      },
      {
        "name": "branchId",
        "label": "Chi nhánh",
        "type": "Guid",
        "optional": true,
        "description": "Chi nhánh áp dụng.",
        "kind": "guid"
      }
    ],
    "body": null
  },
  {
    "id": "PricingPolicies.GetAsync",
    "group": "PricingPolicies",
    "groupLabel": "Chính sách giá",
    "label": "Chi tiết",
    "method": "GET",
    "path": "pricing-policies/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "PricingPolicies.GetProductAddOnsAsync",
    "group": "PricingPolicies",
    "groupLabel": "Chính sách giá",
    "label": "Món kèm",
    "method": "POST",
    "path": "pricing-policies/product-addons",
    "pathFields": [],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "PricingPoliciesGetProductAddOnsBody",
      "type": "PricingPoliciesGetProductAddOnsBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "PricingPolicies.CreateAsync",
    "group": "PricingPolicies",
    "groupLabel": "Chính sách giá",
    "label": "Tạo mới",
    "method": "POST",
    "path": "pricing-policies",
    "pathFields": [],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "PricingPolicyRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "PricingPolicyRequest"
    }
  },
  {
    "id": "PricingPolicies.UpdateAsync",
    "group": "PricingPolicies",
    "groupLabel": "Chính sách giá",
    "label": "Cập nhật",
    "method": "PUT",
    "path": "pricing-policies/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "PricingPolicyRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "PricingPolicyRequest"
    }
  },
  {
    "id": "PricingPolicies.DeleteAsync",
    "group": "PricingPolicies",
    "groupLabel": "Chính sách giá",
    "label": "Xóa",
    "method": "POST",
    "path": "pricing-policies/{id}/delete",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "SaleChannels.GetOptionsAsync",
    "group": "SaleChannels",
    "groupLabel": "Kênh bán",
    "label": "Kênh bán có thể thêm",
    "method": "GET",
    "path": "sale-channels/options",
    "pathFields": [],
    "queryFields": [],
    "body": null
  },
  {
    "id": "SaleChannels.ListAsync",
    "group": "SaleChannels",
    "groupLabel": "Kênh bán",
    "label": "Danh sách",
    "method": "GET",
    "path": "sale-channels",
    "pathFields": [],
    "queryFields": [
      {
        "name": "pageNumber",
        "label": "Trang",
        "type": "int",
        "optional": false,
        "description": "Trang cần lấy, bắt đầu từ 1. Mặc định 1.",
        "kind": "number",
        "default": 1
      },
      {
        "name": "pageSize",
        "label": "Số dòng / trang",
        "type": "int",
        "optional": false,
        "description": "Số phần tử mỗi trang, nên từ 1 đến 100. Mặc định 15.",
        "kind": "number",
        "default": 15
      },
      {
        "name": "name",
        "label": "Tên",
        "type": "string",
        "optional": true,
        "description": "Tên kênh bán.",
        "kind": "string"
      },
      {
        "name": "isActive",
        "label": "Đang hoạt động",
        "type": "bool",
        "optional": true,
        "description": "Trạng thái hoạt động.",
        "kind": "boolean"
      }
    ],
    "body": null
  },
  {
    "id": "SaleChannels.GetAsync",
    "group": "SaleChannels",
    "groupLabel": "Kênh bán",
    "label": "Chi tiết",
    "method": "GET",
    "path": "sale-channels/{saleChannelId}",
    "pathFields": [
      {
        "name": "saleChannelId",
        "label": "Kênh bán",
        "type": "string",
        "optional": false,
        "description": "",
        "kind": "string"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "SaleChannels.CreateAsync",
    "group": "SaleChannels",
    "groupLabel": "Kênh bán",
    "label": "Tạo mới",
    "method": "POST",
    "path": "sale-channels",
    "pathFields": [],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "SaleChannelCreateRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "SaleChannelCreateRequest"
    }
  },
  {
    "id": "SaleChannels.DeleteAsync",
    "group": "SaleChannels",
    "groupLabel": "Kênh bán",
    "label": "Xóa",
    "method": "POST",
    "path": "sale-channels/{saleChannelId}/delete",
    "pathFields": [
      {
        "name": "saleChannelId",
        "label": "Kênh bán",
        "type": "string",
        "optional": false,
        "description": "",
        "kind": "string"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "Combos.ListAsync",
    "group": "Combos",
    "groupLabel": "Combo",
    "label": "Danh sách",
    "method": "GET",
    "path": "combos",
    "pathFields": [],
    "queryFields": [
      {
        "name": "pageNumber",
        "label": "Trang",
        "type": "int",
        "optional": false,
        "description": "Trang cần lấy, bắt đầu từ 1. Mặc định 1.",
        "kind": "number",
        "default": 1
      },
      {
        "name": "pageSize",
        "label": "Số dòng / trang",
        "type": "int",
        "optional": false,
        "description": "Số phần tử mỗi trang, nên từ 1 đến 100. Mặc định 15.",
        "kind": "number",
        "default": 15
      },
      {
        "name": "key",
        "label": "Từ khóa",
        "type": "string",
        "optional": true,
        "description": "Từ khóa theo tên.",
        "kind": "string"
      },
      {
        "name": "isActive",
        "label": "Đang hoạt động",
        "type": "bool",
        "optional": true,
        "description": "Trạng thái hoạt động.",
        "kind": "boolean"
      }
    ],
    "body": null
  },
  {
    "id": "Combos.GetAsync",
    "group": "Combos",
    "groupLabel": "Combo",
    "label": "Chi tiết",
    "method": "GET",
    "path": "combos/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "Combos.CreateAsync",
    "group": "Combos",
    "groupLabel": "Combo",
    "label": "Tạo mới",
    "method": "POST",
    "path": "combos",
    "pathFields": [],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "ComboRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "ComboRequest"
    }
  },
  {
    "id": "Combos.UpdateAsync",
    "group": "Combos",
    "groupLabel": "Combo",
    "label": "Cập nhật",
    "method": "PUT",
    "path": "combos/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "ComboRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "ComboRequest"
    }
  },
  {
    "id": "Combos.DeleteAsync",
    "group": "Combos",
    "groupLabel": "Combo",
    "label": "Xóa",
    "method": "POST",
    "path": "combos/{id}/delete",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "FlashSales.ListAsync",
    "group": "FlashSales",
    "groupLabel": "Flash sale",
    "label": "Danh sách",
    "method": "GET",
    "path": "flashsales",
    "pathFields": [],
    "queryFields": [
      {
        "name": "pageNumber",
        "label": "Trang",
        "type": "int",
        "optional": false,
        "description": "Trang cần lấy, bắt đầu từ 1. Mặc định 1.",
        "kind": "number",
        "default": 1
      },
      {
        "name": "pageSize",
        "label": "Số dòng / trang",
        "type": "int",
        "optional": false,
        "description": "Số phần tử mỗi trang, nên từ 1 đến 100. Mặc định 15.",
        "kind": "number",
        "default": 15
      },
      {
        "name": "key",
        "label": "Từ khóa",
        "type": "string",
        "optional": true,
        "description": "Từ khóa theo tên.",
        "kind": "string"
      },
      {
        "name": "isActive",
        "label": "Đang hoạt động",
        "type": "bool",
        "optional": true,
        "description": "Trạng thái hoạt động.",
        "kind": "boolean"
      },
      {
        "name": "status",
        "label": "Trạng thái",
        "type": "FlashSaleStatusFilter",
        "optional": true,
        "description": "Trạng thái chạy. Mặc định tất cả.",
        "kind": "enum",
        "options": [
          {
            "label": "All",
            "value": 0
          },
          {
            "label": "Running",
            "value": 1
          },
          {
            "label": "Paused",
            "value": 2
          },
          {
            "label": "Upcoming",
            "value": 3
          },
          {
            "label": "Ended",
            "value": 4
          }
        ]
      },
      {
        "name": "startDate",
        "label": "Bắt đầu",
        "type": "DateTime",
        "optional": true,
        "description": "Từ thời điểm.",
        "kind": "datetime"
      },
      {
        "name": "endDate",
        "label": "Kết thúc",
        "type": "DateTime",
        "optional": true,
        "description": "Đến thời điểm.",
        "kind": "datetime"
      }
    ],
    "body": null
  },
  {
    "id": "FlashSales.GetAsync",
    "group": "FlashSales",
    "groupLabel": "Flash sale",
    "label": "Chi tiết",
    "method": "GET",
    "path": "flashsales/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "FlashSales.CreateAsync",
    "group": "FlashSales",
    "groupLabel": "Flash sale",
    "label": "Tạo mới",
    "method": "POST",
    "path": "flashsales",
    "pathFields": [],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "FlashSaleRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "FlashSaleRequest"
    }
  },
  {
    "id": "FlashSales.UpdateAsync",
    "group": "FlashSales",
    "groupLabel": "Flash sale",
    "label": "Cập nhật",
    "method": "PUT",
    "path": "flashsales/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "FlashSaleRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "FlashSaleRequest"
    }
  },
  {
    "id": "FlashSales.DeleteAsync",
    "group": "FlashSales",
    "groupLabel": "Flash sale",
    "label": "Xóa",
    "method": "POST",
    "path": "flashsales/{id}/delete",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "Vouchers.ListAsync",
    "group": "Vouchers",
    "groupLabel": "Voucher",
    "label": "Danh sách",
    "method": "GET",
    "path": "vouchers",
    "pathFields": [],
    "queryFields": [
      {
        "name": "pageNumber",
        "label": "Trang",
        "type": "int",
        "optional": false,
        "description": "Trang cần lấy, bắt đầu từ 1. Mặc định 1.",
        "kind": "number",
        "default": 1
      },
      {
        "name": "pageSize",
        "label": "Số dòng / trang",
        "type": "int",
        "optional": false,
        "description": "Số phần tử mỗi trang, nên từ 1 đến 100. Mặc định 15.",
        "kind": "number",
        "default": 15
      },
      {
        "name": "isAvailable",
        "label": "Đang khả dụng",
        "type": "bool",
        "optional": false,
        "description": "true : chỉ voucher còn hiệu lực; false (mặc định): tất cả.",
        "kind": "boolean"
      },
      {
        "name": "fromDate",
        "label": "Từ ngày",
        "type": "DateTime",
        "optional": true,
        "description": "Từ ngày.",
        "kind": "datetime"
      },
      {
        "name": "toDate",
        "label": "Đến ngày",
        "type": "DateTime",
        "optional": true,
        "description": "Đến ngày.",
        "kind": "datetime"
      },
      {
        "name": "title",
        "label": "Tên chương trình",
        "type": "string",
        "optional": true,
        "description": "Tiêu đề.",
        "kind": "string"
      }
    ],
    "body": null
  },
  {
    "id": "Vouchers.GetAsync",
    "group": "Vouchers",
    "groupLabel": "Voucher",
    "label": "Chi tiết",
    "method": "GET",
    "path": "vouchers/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "Vouchers.GetCodesAsync",
    "group": "Vouchers",
    "groupLabel": "Voucher",
    "label": "Danh sách mã voucher",
    "method": "GET",
    "path": "vouchers/{id}/codes",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [
      {
        "name": "pageNumber",
        "label": "Trang",
        "type": "int",
        "optional": false,
        "description": "Trang cần lấy, bắt đầu từ 1. Mặc định 1.",
        "kind": "number",
        "default": 1
      },
      {
        "name": "pageSize",
        "label": "Số dòng / trang",
        "type": "int",
        "optional": false,
        "description": "Số phần tử mỗi trang, nên từ 1 đến 100. Mặc định 15.",
        "kind": "number",
        "default": 15
      },
      {
        "name": "isUsed",
        "label": "Đã sử dụng",
        "type": "bool",
        "optional": false,
        "description": "true : liệt kê từng lần mã được dùng trong đơn; false (mặc định): liệt kê mã.",
        "kind": "boolean"
      },
      {
        "name": "isAvailable",
        "label": "Đang khả dụng",
        "type": "bool",
        "optional": false,
        "description": "Khi là false : true chỉ lấy mã chưa dùng.",
        "kind": "boolean"
      }
    ],
    "body": null
  },
  {
    "id": "Vouchers.CreateAsync",
    "group": "Vouchers",
    "groupLabel": "Voucher",
    "label": "Tạo mới",
    "method": "POST",
    "path": "vouchers",
    "pathFields": [],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "VoucherRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "VoucherRequest"
    }
  },
  {
    "id": "Vouchers.UpdateAsync",
    "group": "Vouchers",
    "groupLabel": "Voucher",
    "label": "Cập nhật",
    "method": "PUT",
    "path": "vouchers/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "VoucherRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "VoucherRequest"
    }
  },
  {
    "id": "Vouchers.SetActiveAsync",
    "group": "Vouchers",
    "groupLabel": "Voucher",
    "label": "Bật / tắt",
    "method": "PUT",
    "path": "vouchers/{id}/active",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "VouchersSetActiveBody",
      "type": "VouchersSetActiveBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "Vouchers.DeleteAsync",
    "group": "Vouchers",
    "groupLabel": "Voucher",
    "label": "Xóa",
    "method": "POST",
    "path": "vouchers/{id}/delete",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "OrderAdjustmentPrograms.ListAsync",
    "group": "OrderAdjustmentPrograms",
    "groupLabel": "Chiết khấu / phụ thu",
    "label": "Danh sách",
    "method": "GET",
    "path": "order-adjustment-programs",
    "pathFields": [],
    "queryFields": [
      {
        "name": "pageNumber",
        "label": "Trang",
        "type": "int",
        "optional": false,
        "description": "Trang cần lấy, bắt đầu từ 1. Mặc định 1.",
        "kind": "number",
        "default": 1
      },
      {
        "name": "pageSize",
        "label": "Số dòng / trang",
        "type": "int",
        "optional": false,
        "description": "Số phần tử mỗi trang, nên từ 1 đến 100. Mặc định 15.",
        "kind": "number",
        "default": 15
      },
      {
        "name": "key",
        "label": "Từ khóa",
        "type": "string",
        "optional": true,
        "description": "Từ khóa theo tên.",
        "kind": "string"
      },
      {
        "name": "isActive",
        "label": "Đang hoạt động",
        "type": "bool",
        "optional": true,
        "description": "Trạng thái hoạt động.",
        "kind": "boolean"
      },
      {
        "name": "type",
        "label": "Loại",
        "type": "OrderAdjustmentType",
        "optional": true,
        "description": "Loại chương trình.",
        "kind": "enum",
        "options": [
          {
            "label": "Discount",
            "value": 1
          },
          {
            "label": "Surcharge",
            "value": 2
          }
        ]
      },
      {
        "name": "branchId",
        "label": "Chi nhánh",
        "type": "Guid",
        "optional": true,
        "description": "Chi nhánh áp dụng.",
        "kind": "guid"
      }
    ],
    "body": null
  },
  {
    "id": "OrderAdjustmentPrograms.GetAsync",
    "group": "OrderAdjustmentPrograms",
    "groupLabel": "Chiết khấu / phụ thu",
    "label": "Chi tiết",
    "method": "GET",
    "path": "order-adjustment-programs/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "OrderAdjustmentPrograms.CreateAsync",
    "group": "OrderAdjustmentPrograms",
    "groupLabel": "Chiết khấu / phụ thu",
    "label": "Tạo mới",
    "method": "POST",
    "path": "order-adjustment-programs",
    "pathFields": [],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "OrderAdjustmentProgramRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "OrderAdjustmentProgramRequest"
    }
  },
  {
    "id": "OrderAdjustmentPrograms.UpdateAsync",
    "group": "OrderAdjustmentPrograms",
    "groupLabel": "Chiết khấu / phụ thu",
    "label": "Cập nhật",
    "method": "PUT",
    "path": "order-adjustment-programs/{id}",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "OrderAdjustmentProgramRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "OrderAdjustmentProgramRequest"
    }
  },
  {
    "id": "OrderAdjustmentPrograms.DeleteAsync",
    "group": "OrderAdjustmentPrograms",
    "groupLabel": "Chiết khấu / phụ thu",
    "label": "Xóa",
    "method": "POST",
    "path": "order-adjustment-programs/{id}/delete",
    "pathFields": [
      {
        "name": "id",
        "label": "ID bản ghi",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "Fnb.GetAreasAsync",
    "group": "Fnb",
    "groupLabel": "Dữ liệu bán FnB",
    "label": "Khu vực & bàn",
    "method": "GET",
    "path": "fnb/areas",
    "pathFields": [],
    "queryFields": [
      {
        "name": "branchId",
        "label": "Chi nhánh",
        "type": "Guid",
        "optional": true,
        "description": "",
        "kind": "guid"
      }
    ],
    "body": null
  },
  {
    "id": "Fnb.GetMenuAsync",
    "group": "Fnb",
    "groupLabel": "Dữ liệu bán FnB",
    "label": "Menu bán hàng",
    "method": "GET",
    "path": "fnb/menu",
    "pathFields": [],
    "queryFields": [
      {
        "name": "branchId",
        "label": "Chi nhánh",
        "type": "Guid",
        "optional": true,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "saleChannelId",
        "label": "Kênh bán",
        "type": "string",
        "optional": true,
        "description": "",
        "kind": "string"
      }
    ],
    "body": null
  },
  {
    "id": "Fnb.GetProductAddOnsAsync",
    "group": "Fnb",
    "groupLabel": "Dữ liệu bán FnB",
    "label": "Món kèm",
    "method": "GET",
    "path": "fnb/products/{productId}/addons",
    "pathFields": [
      {
        "name": "productId",
        "label": "Sản phẩm",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [
      {
        "name": "branchId",
        "label": "Chi nhánh",
        "type": "Guid",
        "optional": true,
        "description": "",
        "kind": "guid"
      }
    ],
    "body": null
  },
  {
    "id": "Fnb.GetProductAttributesAsync",
    "group": "Fnb",
    "groupLabel": "Dữ liệu bán FnB",
    "label": "Thuộc tính món",
    "method": "GET",
    "path": "fnb/products/{productId}/attributes",
    "pathFields": [
      {
        "name": "productId",
        "label": "Sản phẩm",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [
      {
        "name": "branchId",
        "label": "Chi nhánh",
        "type": "Guid",
        "optional": true,
        "description": "",
        "kind": "guid"
      }
    ],
    "body": null
  },
  {
    "id": "Fnb.GetPaymentOptionsAsync",
    "group": "Fnb",
    "groupLabel": "Dữ liệu bán FnB",
    "label": "Phương thức thanh toán",
    "method": "GET",
    "path": "fnb/payment-options",
    "pathFields": [],
    "queryFields": [
      {
        "name": "branchId",
        "label": "Chi nhánh",
        "type": "Guid",
        "optional": true,
        "description": "",
        "kind": "guid"
      }
    ],
    "body": null
  },
  {
    "id": "FnbOrders.CreateAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Tạo mới",
    "method": "POST",
    "path": "fnb/orders",
    "pathFields": [],
    "queryFields": [
      {
        "name": "branchId",
        "label": "Chi nhánh",
        "type": "Guid",
        "optional": true,
        "description": "",
        "kind": "guid"
      }
    ],
    "body": {
      "name": "body",
      "label": "body",
      "type": "FnbOrderCreateRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "FnbOrderCreateRequest"
    }
  },
  {
    "id": "FnbOrders.ListAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Danh sách",
    "method": "GET",
    "path": "fnb/orders",
    "pathFields": [],
    "queryFields": [
      {
        "name": "pageNumber",
        "label": "Trang",
        "type": "int",
        "optional": false,
        "description": "Trang cần lấy, bắt đầu từ 1. Mặc định 1.",
        "kind": "number",
        "default": 1
      },
      {
        "name": "pageSize",
        "label": "Số dòng / trang",
        "type": "int",
        "optional": false,
        "description": "Số phần tử mỗi trang, nên từ 1 đến 100. Mặc định 15.",
        "kind": "number",
        "default": 15
      },
      {
        "name": "branchId",
        "label": "Chi nhánh",
        "type": "Guid",
        "optional": true,
        "description": "Chi nhánh; bỏ trống dùng chi nhánh mặc định của đối tác.",
        "kind": "guid"
      },
      {
        "name": "fromDate",
        "label": "Từ ngày",
        "type": "DateTime",
        "optional": true,
        "description": "Từ thời điểm (giờ Việt Nam).",
        "kind": "datetime"
      },
      {
        "name": "toDate",
        "label": "Đến ngày",
        "type": "DateTime",
        "optional": true,
        "description": "Đến thời điểm (giờ Việt Nam).",
        "kind": "datetime"
      },
      {
        "name": "isPaid",
        "label": "Đã thanh toán",
        "type": "bool",
        "optional": true,
        "description": "null tất cả, true đã thanh toán, false chưa thanh toán.",
        "kind": "boolean"
      },
      {
        "name": "orderCode",
        "label": "Mã đơn",
        "type": "string",
        "optional": true,
        "description": "Mã đơn hàng.",
        "kind": "string"
      },
      {
        "name": "keyWord",
        "label": "Từ khóa",
        "type": "string",
        "optional": true,
        "description": "Tên hoặc số điện thoại khách hàng.",
        "kind": "string"
      }
    ],
    "body": null
  },
  {
    "id": "FnbOrders.GetAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Chi tiết",
    "method": "GET",
    "path": "fnb/orders/{orderId}",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "FnbOrders.UpdateNoteAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Ghi chú đơn",
    "method": "PUT",
    "path": "fnb/orders/{orderId}/note",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "FnbOrdersUpdateNoteBody",
      "type": "FnbOrdersUpdateNoteBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "FnbOrders.SetCustomerAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Khách của đơn",
    "method": "PUT",
    "path": "fnb/orders/{orderId}/customer",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "FnbOrdersSetCustomerBody",
      "type": "FnbOrdersSetCustomerBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "FnbOrders.CancelAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Hủy đơn",
    "method": "POST",
    "path": "fnb/orders/{orderId}/cancel",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "FnbOrdersCancelBody",
      "type": "FnbOrdersCancelBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "FnbOrders.AddProductAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Thêm một món",
    "method": "POST",
    "path": "fnb/orders/{orderId}/products",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "FnbOrderItemRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "FnbOrderItemRequest"
    }
  },
  {
    "id": "FnbOrders.AddProductsAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Thêm nhiều món",
    "method": "POST",
    "path": "fnb/orders/{orderId}/products/bulk",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "IEnumerable<FnbOrderItemRequest>",
      "optional": false,
      "description": "",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "FnbOrderItemRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "FnbOrderItemRequest"
      }
    }
  },
  {
    "id": "FnbOrders.UpdateProductQuantityAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Sửa số lượng món",
    "method": "PUT",
    "path": "fnb/orders/{orderId}/products/{lineId}/quantity",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "lineId",
        "label": "Dòng món / combo",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "FnbOrdersUpdateProductQuantityBody",
      "type": "FnbOrdersUpdateProductQuantityBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "FnbOrders.UpdateProductUnitAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Đổi đơn vị tính",
    "method": "PUT",
    "path": "fnb/orders/{orderId}/products/{lineId}/unit",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "lineId",
        "label": "Dòng món / combo",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "FnbOrdersUpdateProductUnitBody",
      "type": "FnbOrdersUpdateProductUnitBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "FnbOrders.UpdateProductPriceAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Đổi giá món",
    "method": "PUT",
    "path": "fnb/orders/{orderId}/products/{lineId}/price",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "lineId",
        "label": "Dòng món / combo",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "FnbOrdersUpdateProductPriceBody",
      "type": "FnbOrdersUpdateProductPriceBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "FnbOrders.UpdateProductNoteAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Ghi chú món",
    "method": "PUT",
    "path": "fnb/orders/{orderId}/products/{lineId}/note",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "lineId",
        "label": "Dòng món / combo",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "FnbOrdersUpdateProductNoteBody",
      "type": "FnbOrdersUpdateProductNoteBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "FnbOrders.UpdateProductAttributesAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Thuộc tính món",
    "method": "PUT",
    "path": "fnb/orders/{orderId}/products/{lineId}/attributes",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "lineId",
        "label": "Dòng món / combo",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "IEnumerable<AttributeValue>",
      "optional": false,
      "description": "",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "AttributeValue",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "AttributeValue"
      }
    }
  },
  {
    "id": "FnbOrders.UpdateProductAddOnsAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Món kèm của dòng",
    "method": "PUT",
    "path": "fnb/orders/{orderId}/products/{lineId}/addons",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "lineId",
        "label": "Dòng món / combo",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "IEnumerable<FnbOrderAddOnRequest>",
      "optional": false,
      "description": "",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "FnbOrderAddOnRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "FnbOrderAddOnRequest"
      }
    }
  },
  {
    "id": "FnbOrders.DeleteProductAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Xóa dòng món",
    "method": "POST",
    "path": "fnb/orders/{orderId}/products/{lineId}/delete",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "lineId",
        "label": "Dòng món / combo",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "FnbOrders.CancelProductAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Hủy món đã chế biến",
    "method": "POST",
    "path": "fnb/orders/{orderId}/products/{lineId}/cancel",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "lineId",
        "label": "Dòng món / combo",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "FnbOrdersCancelProductBody",
      "type": "FnbOrdersCancelProductBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "FnbOrders.AddComboAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Thêm một combo",
    "method": "POST",
    "path": "fnb/orders/{orderId}/combos",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "FnbOrderComboRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "FnbOrderComboRequest"
    }
  },
  {
    "id": "FnbOrders.AddCombosAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Thêm nhiều combo",
    "method": "POST",
    "path": "fnb/orders/{orderId}/combos/bulk",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "IEnumerable<FnbOrderComboRequest>",
      "optional": false,
      "description": "",
      "kind": "array",
      "item": {
        "name": "",
        "label": "",
        "type": "FnbOrderComboRequest",
        "optional": false,
        "description": "",
        "kind": "object",
        "ref": "FnbOrderComboRequest"
      }
    }
  },
  {
    "id": "FnbOrders.UpdateComboAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Sửa dòng combo",
    "method": "PUT",
    "path": "fnb/orders/{orderId}/combos/{lineId}",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "lineId",
        "label": "Dòng món / combo",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "FnbOrderComboRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "FnbOrderComboRequest"
    }
  },
  {
    "id": "FnbOrders.DeleteComboAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Xóa dòng combo",
    "method": "POST",
    "path": "fnb/orders/{orderId}/combos/{lineId}/delete",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "lineId",
        "label": "Dòng món / combo",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": null
  },
  {
    "id": "FnbOrders.CancelComboAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Hủy combo",
    "method": "POST",
    "path": "fnb/orders/{orderId}/combos/{lineId}/cancel",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "lineId",
        "label": "Dòng món / combo",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "FnbOrdersCancelComboBody",
      "type": "FnbOrdersCancelComboBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "FnbOrders.ChangeTableAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Đổi bàn / kênh bán",
    "method": "PUT",
    "path": "fnb/orders/{orderId}/table-sale-channel",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "FnbOrdersChangeTableBody",
      "type": "FnbOrdersChangeTableBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "FnbOrders.MergeAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Gộp đơn",
    "method": "POST",
    "path": "fnb/orders/merge",
    "pathFields": [],
    "queryFields": [],
    "body": {
      "kind": "object",
      "ref": "FnbOrdersMergeBody",
      "type": "FnbOrdersMergeBody",
      "name": "body",
      "label": "Dữ liệu gửi",
      "optional": false
    }
  },
  {
    "id": "FnbOrders.MoveItemsAsync",
    "group": "FnbOrders",
    "groupLabel": "Đơn hàng FnB",
    "label": "Chuyển / tách món",
    "method": "POST",
    "path": "fnb/orders/{orderId}/move-items",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "FnbMoveItemsRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "FnbMoveItemsRequest"
    }
  },
  {
    "id": "FnbPayments.PreviewAsync",
    "group": "FnbPayments",
    "groupLabel": "Thanh toán & QR",
    "label": "Tạm tính",
    "method": "POST",
    "path": "fnb/orders/{orderId}/payment/preview",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "FnbPaymentRequest",
      "optional": true,
      "description": "",
      "kind": "object",
      "ref": "FnbPaymentRequest"
    }
  },
  {
    "id": "FnbPayments.CreateQrAsync",
    "group": "FnbPayments",
    "groupLabel": "Thanh toán & QR",
    "label": "Tạo QR thanh toán",
    "method": "POST",
    "path": "fnb/orders/{orderId}/qr",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "FnbPaymentQrRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "FnbPaymentQrRequest"
    }
  },
  {
    "id": "FnbPayments.CheckQrTransactionAsync",
    "group": "FnbPayments",
    "groupLabel": "Thanh toán & QR",
    "label": "Kiểm tra chuyển khoản",
    "method": "GET",
    "path": "fnb/orders/{orderId}/qr/check/{provider}",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      },
      {
        "name": "provider",
        "label": "Nhà cung cấp QR",
        "type": "string",
        "optional": false,
        "description": "",
        "kind": "string"
      }
    ],
    "queryFields": [
      {
        "name": "transactionId",
        "label": "Mã giao dịch",
        "type": "Guid",
        "optional": true,
        "description": "",
        "kind": "guid"
      }
    ],
    "body": null
  },
  {
    "id": "FnbPayments.PayAsync",
    "group": "FnbPayments",
    "groupLabel": "Thanh toán & QR",
    "label": "Ghi nhận thanh toán",
    "method": "PUT",
    "path": "fnb/orders/{orderId}/payment",
    "pathFields": [
      {
        "name": "orderId",
        "label": "Đơn hàng",
        "type": "Guid",
        "optional": false,
        "description": "",
        "kind": "guid"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "body",
      "type": "FnbPaymentRequest",
      "optional": false,
      "description": "",
      "kind": "object",
      "ref": "FnbPaymentRequest"
    }
  },
  {
    "id": "SaleChannels.UpdateAsync",
    "group": "SaleChannels",
    "groupLabel": "Kênh bán",
    "label": "Cập nhật",
    "method": "PUT",
    "path": "sale-channels/{saleChannelId}",
    "pathFields": [
      {
        "name": "saleChannelId",
        "label": "Kênh bán",
        "type": "string",
        "optional": false,
        "description": "ID kênh bán lấy từ tham số đường dẫn.",
        "kind": "string"
      }
    ],
    "queryFields": [],
    "body": {
      "name": "body",
      "label": "Dữ liệu gửi",
      "type": "SaleChannelUpdateRequest",
      "optional": false,
      "kind": "object",
      "ref": "SaleChannelUpdateRequest"
    }
  }
];
