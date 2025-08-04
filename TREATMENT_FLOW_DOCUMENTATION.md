# 诊疗流程管理系统文档

## 概述

本系统实现了完整的诊疗流程跟踪功能，从患者预约到治疗完成的每个阶段都有详细的管理和监控。

## 主要功能

### 1. 完整诊疗流程跟踪

#### API端点：
- `GET /api/TreatmentFlow/patient/{customerId}` - 获取患者的完整诊疗流程
- `GET /api/TreatmentTracking/patient/{customerId}/complete-flow` - 获取完整的治疗跟踪信息

#### 功能特点：
- 显示所有治疗计划
- 包含预约记录
- 医疗记录历史
- 检查结果记录
- 治疗进度统计

### 2. 治疗阶段管理

#### API端点：
- `PUT /api/TreatmentFlow/treatment-plan/{id}/phase` - 更新治疗阶段
- `GET /api/TreatmentTracking/treatment-plan/{treatmentPlanId}/phase-details` - 获取阶段详情
- `POST /api/TreatmentTracking/treatment-plan/{treatmentPlanId}/progress-update` - 更新治疗进度

#### 支持的治疗类型：
- **IVF (试管婴儿)**
  - 阶段1: 健康检查和基础检查
  - 阶段2: 卵巢刺激
  - 阶段3: 取卵手术
  - 阶段4: 体外受精
  - 阶段5: 胚胎移植
  - 阶段6: 妊娠监测

- **IUI (人工授精)**
  - 阶段1: 健康检查
  - 阶段2: 排卵刺激
  - 阶段3: 人工授精
  - 阶段4: 妊娠监测

### 3. 医疗记录管理

#### API端点：
- `POST /api/TreatmentFlow/medical-record` - 创建医疗记录
- `POST /api/TreatmentFlow/test-result` - 记录检查结果

#### 记录内容包括：
- 症状描述
- 诊断结果
- 治疗方案
- 处方信息
- 医生备注

### 4. 治疗提醒和日程安排

#### API端点：
- `GET /api/TreatmentFlow/reminders/{customerId}` - 获取治疗提醒
- `GET /api/TreatmentTracking/patient/{customerId}/reminders` - 获取患者提醒

#### 提醒类型：
- 预约提醒
- 治疗阶段提醒
- 检查日期提醒
- 用药提醒

### 5. 治疗统计和进度

#### API端点：
- `GET /api/TreatmentFlow/progress-stats/{customerId}` - 获取治疗进度统计
- `GET /api/TreatmentTracking/patient/{customerId}/statistics` - 获取治疗统计

#### 统计信息：
- 治疗计划状态分布
- 费用统计
- 治疗天数
- 进度百分比

### 6. 治疗时间线

#### API端点：
- `GET /api/TreatmentFlow/timeline/{customerId}` - 获取治疗时间线
- `GET /api/TreatmentTracking/patient/{customerId}/timeline` - 获取患者时间线

#### 时间线内容包括：
- 预约记录
- 医疗记录
- 检查结果
- 按时间顺序排列

## 数据模型

### TreatmentPlan (治疗计划)
```csharp
public class TreatmentPlan
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int DoctorId { get; set; }
    public string TreatmentType { get; set; } // IVF, IUI, etc.
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } // Active, Completed, Cancelled
    public int CurrentPhase { get; set; }
    public string PhaseDescription { get; set; }
    public DateTime? NextPhaseDate { get; set; }
    public DateTime? NextVisitDate { get; set; }
    public decimal TotalCost { get; set; }
    public decimal PaidAmount { get; set; }
    public string PaymentStatus { get; set; }
    public string Notes { get; set; }
    public string ProgressNotes { get; set; }
}
```

### MedicalRecord (医疗记录)
```csharp
public class MedicalRecord
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int DoctorId { get; set; }
    public int AppointmentId { get; set; }
    public string Symptoms { get; set; }
    public string Diagnosis { get; set; }
    public string Treatment { get; set; }
    public string Prescription { get; set; }
    public string Notes { get; set; }
    public DateTime RecordDate { get; set; }
}
```

### TestResult (检查结果)
```csharp
public class TestResult
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int DoctorId { get; set; }
    public string TestName { get; set; }
    public string TestType { get; set; }
    public string Result { get; set; }
    public string ReferenceRange { get; set; }
    public string Status { get; set; } // Normal, Abnormal, Critical
    public DateTime TestDate { get; set; }
}
```

## 使用示例

### 1. 获取患者完整诊疗流程
```http
GET /api/TreatmentFlow/patient/1
Authorization: Bearer {token}
```

### 2. 更新治疗阶段
```http
PUT /api/TreatmentFlow/treatment-plan/1/phase
Authorization: Bearer {token}
Content-Type: application/json

{
    "currentPhase": 2,
    "phaseDescription": "Kích thích buồng trứng",
    "nextPhaseDate": "2024-01-15T00:00:00",
    "nextVisitDate": "2024-01-10T09:00:00",
    "progressNotes": "Bắt đầu tiêm thuốc kích thích buồng trứng",
    "notes": "Theo dõi phản ứng với thuốc"
}
```

### 3. 创建医疗记录
```http
POST /api/TreatmentFlow/medical-record
Authorization: Bearer {token}
Content-Type: application/json

{
    "customerId": 1,
    "doctorId": 1,
    "appointmentId": 1,
    "symptoms": "Khó thụ thai trong 2 năm",
    "diagnosis": "Hiếm muộn không rõ nguyên nhân",
    "treatment": "Điều trị IVF",
    "prescription": "Thuốc kích thích buồng trứng",
    "notes": "Bệnh nhân cần theo dõi chặt chẽ"
}
```

## 权限控制

- **Customer**: 可以查看自己的治疗流程、提醒和统计
- **Doctor**: 可以更新治疗阶段、创建医疗记录和检查结果
- **Admin**: 拥有所有权限

## 错误处理

系统提供详细的错误信息：
- 400: 请求数据无效
- 401: 未授权访问
- 403: 权限不足
- 404: 资源不存在
- 500: 服务器内部错误

## 安全特性

- JWT身份验证
- 基于角色的权限控制
- 数据验证和清理
- SQL注入防护
- XSS防护

## 性能优化

- 使用Entity Framework的Include优化查询
- 分页加载大量数据
- 缓存常用数据
- 异步处理

## 扩展功能

### 未来计划：
1. 实时通知系统
2. 移动端推送
3. 视频咨询集成
4. 电子处方系统
5. 智能提醒算法
6. 数据分析报告

这个诊疗流程管理系统为不孕症治疗提供了完整的数字化解决方案，确保每个治疗阶段都能得到妥善管理和监控。 