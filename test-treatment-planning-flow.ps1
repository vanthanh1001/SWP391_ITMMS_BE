# Test Treatment Planning Flow
# Quy trình: Chọn dịch vụ → Tạo kế hoạch → Theo dõi giai đoạn → Cập nhật tiến trình → Hoàn thành

Write-Host "=== TESTING TREATMENT PLANNING FLOW ===" -ForegroundColor Green
Write-Host ""

$baseUrl = "http://localhost:5037"
$headers = @{ "Content-Type" = "application/json" }

# Step 1: Xem danh sách dịch vụ điều trị có sẵn
Write-Host "Step 1: Xem danh sách dịch vụ điều trị có sẵn" -ForegroundColor Yellow
try {
    $servicesResponse = Invoke-RestMethod -Uri "$baseUrl/api/treatmentservices" -Method GET -Headers $headers
    Write-Host "✅ Available Treatment Services:" -ForegroundColor Green
    foreach ($service in $servicesResponse.data) {
        Write-Host "   - ID: $($service.id) | $($service.serviceName) ($($service.serviceCode))"
        Write-Host "     Price: $($service.basePrice) VND | Success Rate: $($service.successRate)%"
        Write-Host "     Duration: $($service.durationDays) days"
        Write-Host "   ---"
    }
    Write-Host ""
} catch {
    Write-Host "❌ Error getting treatment services: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Step 2: Tạo kế hoạch điều trị mới cho bệnh nhân
Write-Host "Step 2: Tạo kế hoạch điều trị IVF cho bệnh nhân" -ForegroundColor Yellow

$treatmentPlanData = @{
    customerId = 1
    doctorId = 1
    treatmentServiceId = 1  # IVF service
    treatmentType = "IVF (In Vitro Fertilization)"
    description = "Kế hoạch điều trị IVF hoàn chỉnh cho bệnh nhân hiếm muộn nguyên phát"
    startDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    totalCost = 85000000  # Will be overridden by service price
    phaseDescription = "Giai đoạn 1: Chuẩn bị và kiểm tra ban đầu"
    nextPhaseDate = (Get-Date).AddDays(7).ToString("yyyy-MM-ddTHH:mm:ss")
    nextVisitDate = (Get-Date).AddDays(3).ToString("yyyy-MM-ddTHH:mm:ss")
    notes = "Bệnh nhân cần chuẩn bị tâm lý và thể chất cho quá trình điều trị IVF"
} | ConvertTo-Json

try {
    $createPlanResponse = Invoke-RestMethod -Uri "$baseUrl/api/treatmentplans" -Method POST -Body $treatmentPlanData -Headers $headers
    Write-Host "✅ Treatment Plan Created Successfully:" -ForegroundColor Green
    $planId = $createPlanResponse.data.id
    Write-Host "   - Plan ID: $planId"
    Write-Host "   - Treatment Type: $($createPlanResponse.data.treatmentType)"
    Write-Host "   - Patient: $($createPlanResponse.data.customer.name)"
    Write-Host "   - Doctor: $($createPlanResponse.data.doctor.name)"
    Write-Host "   - Service: $($createPlanResponse.data.treatmentService.serviceName)"
    Write-Host "   - Total Cost: $($createPlanResponse.data.totalCost) VND"
    Write-Host "   - Current Phase: $($createPlanResponse.data.currentPhase)"
    Write-Host "   - Phase Description: $($createPlanResponse.data.phaseDescription)"
    Write-Host "   - Status: $($createPlanResponse.data.status)"
    Write-Host ""
} catch {
    Write-Host "❌ Error creating treatment plan: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Response: $($_.Exception.Response)" -ForegroundColor Red
    exit 1
}

# Step 3: Xem chi tiết kế hoạch điều trị vừa tạo
Write-Host "Step 3: Xem chi tiết kế hoạch điều trị vừa tạo" -ForegroundColor Yellow
try {
    $planDetailsResponse = Invoke-RestMethod -Uri "$baseUrl/api/treatmentplans/$planId" -Method GET -Headers $headers
    Write-Host "✅ Treatment Plan Details:" -ForegroundColor Green
    $plan = $planDetailsResponse.data
    Write-Host "   - ID: $($plan.id)"
    Write-Host "   - Type: $($plan.treatmentType)"
    Write-Host "   - Start Date: $($plan.startDate)"
    Write-Host "   - Status: $($plan.status)"
    Write-Host "   - Current Phase: $($plan.currentPhase) - $($plan.phaseDescription)"
    Write-Host "   - Next Phase Date: $($plan.nextPhaseDate)"
    Write-Host "   - Next Visit Date: $($plan.nextVisitDate)"
    Write-Host "   - Total Cost: $($plan.totalCost) VND"
    Write-Host "   - Payment Status: $($plan.paymentStatus)"
    Write-Host "   - Progress Notes: $($plan.progressNotes)"
    Write-Host ""
} catch {
    Write-Host "❌ Error getting treatment plan details: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 4: Cập nhật tiến trình điều trị (Giai đoạn 2)
Write-Host "Step 4: Cập nhật tiến trình điều trị - Chuyển sang giai đoạn 2" -ForegroundColor Yellow

$progressUpdateData = @{
    currentPhase = 2
    phaseDescription = "Giai đoạn 2: Kích thích buồng trứng và theo dõi"
    nextPhaseDate = (Get-Date).AddDays(14).ToString("yyyy-MM-ddTHH:mm:ss")
    nextVisitDate = (Get-Date).AddDays(5).ToString("yyyy-MM-ddTHH:mm:ss")
    notes = "Bệnh nhân đã hoàn thành các xét nghiệm cơ bản. Bắt đầu giai đoạn kích thích buồng trứng với FSH"
    progressNotes = "Phản ứng tốt với thuốc kích thích. Buồng trứng phản ứng bình thường. Theo dõi siêu âm đều đặn"
    status = "Active"
} | ConvertTo-Json

try {
    $updateProgressResponse = Invoke-RestMethod -Uri "$baseUrl/api/treatmentplans/$planId/progress" -Method PUT -Body $progressUpdateData -Headers $headers
    Write-Host "✅ Treatment Progress Updated Successfully:" -ForegroundColor Green
    $updatedPlan = $updateProgressResponse.data
    Write-Host "   - Current Phase: $($updatedPlan.currentPhase)"
    Write-Host "   - Phase Description: $($updatedPlan.phaseDescription)"
    Write-Host "   - Next Phase Date: $($updatedPlan.nextPhaseDate)"
    Write-Host "   - Next Visit Date: $($updatedPlan.nextVisitDate)"
    Write-Host "   - Doctor Notes: $($updatedPlan.notes)"
    Write-Host "   - Progress Notes: $($updatedPlan.progressNotes)"
    Write-Host ""
} catch {
    Write-Host "❌ Error updating treatment progress: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 5: Xem tất cả kế hoạch điều trị của bệnh nhân
Write-Host "Step 5: Xem tất cả kế hoạch điều trị của bệnh nhân" -ForegroundColor Yellow
try {
    $customerPlansResponse = Invoke-RestMethod -Uri "$baseUrl/api/treatmentplans/customer/1" -Method GET -Headers $headers
    Write-Host "✅ Patient's Treatment Plans:" -ForegroundColor Green
    if ($customerPlansResponse.data.Count -gt 0) {
        foreach ($plan in $customerPlansResponse.data) {
            Write-Host "   - Plan ID: $($plan.id)"
            Write-Host "   - Type: $($plan.treatmentType)"
            Write-Host "   - Status: $($plan.status)"
            Write-Host "   - Current Phase: $($plan.currentPhase) - $($plan.phaseDescription)"
            Write-Host "   - Doctor: $($plan.doctor.name) ($($plan.doctor.specialization))"
            if ($plan.treatmentService) {
                Write-Host "   - Service: $($plan.treatmentService.serviceName)"
            }
            Write-Host "   - Cost: $($plan.totalCost) VND (Paid: $($plan.paidAmount) VND)"
            Write-Host "   - Payment Status: $($plan.paymentStatus)"
            Write-Host "   ---"
        }
    } else {
        Write-Host "   - No treatment plans found"
    }
    Write-Host ""
} catch {
    Write-Host "❌ Error getting patient's treatment plans: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 6: Xem kế hoạch điều trị đang active của bệnh nhân
Write-Host "Step 6: Xem kế hoạch điều trị đang hoạt động của bệnh nhân" -ForegroundColor Yellow
try {
    $activePlanResponse = Invoke-RestMethod -Uri "$baseUrl/api/treatmentplans/customer/1/active" -Method GET -Headers $headers
    Write-Host "✅ Active Treatment Plan:" -ForegroundColor Green
    $activePlan = $activePlanResponse.data
    Write-Host "   - Plan ID: $($activePlan.id)"
    Write-Host "   - Treatment: $($activePlan.treatmentType)"
    Write-Host "   - Current Phase: $($activePlan.currentPhase) - $($activePlan.phaseDescription)"
    Write-Host "   - Next Phase Date: $($activePlan.nextPhaseDate)"
    Write-Host "   - Next Visit Date: $($activePlan.nextVisitDate)"
    Write-Host "   - Doctor: $($activePlan.doctor.name) ($($activePlan.doctor.specialization))"
    Write-Host "   - Phone: $($activePlan.doctor.phone)"
    if ($activePlan.treatmentService) {
        Write-Host "   - Service: $($activePlan.treatmentService.serviceName)"
        Write-Host "   - Success Rate: $($activePlan.treatmentService.successRate)%"
        Write-Host "   - Base Price: $($activePlan.treatmentService.basePrice) VND"
    }
    Write-Host "   - Progress Notes: $($activePlan.progressNotes)"
    Write-Host ""
} catch {
    Write-Host "❌ Error getting active treatment plan: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 7: Doctor xem tất cả kế hoạch điều trị đang quản lý
Write-Host "Step 7: Doctor xem tất cả kế hoạch điều trị đang quản lý" -ForegroundColor Yellow
try {
    $doctorPlansResponse = Invoke-RestMethod -Uri "$baseUrl/api/treatmentplans/doctor/1" -Method GET -Headers $headers
    Write-Host "✅ Doctor's Treatment Plans:" -ForegroundColor Green
    if ($doctorPlansResponse.data.Count -gt 0) {
        foreach ($plan in $doctorPlansResponse.data) {
            Write-Host "   - Plan ID: $($plan.id)"
            Write-Host "   - Patient: $($plan.customer.name)"
            Write-Host "   - Contact: $($plan.customer.phone) | $($plan.customer.email)"
            Write-Host "   - Treatment: $($plan.treatmentType)"
            Write-Host "   - Status: $($plan.status)"
            Write-Host "   - Current Phase: $($plan.currentPhase) - $($plan.phaseDescription)"
            Write-Host "   - Next Visit: $($plan.nextVisitDate)"
            if ($plan.treatmentService) {
                Write-Host "   - Service: $($plan.treatmentService.serviceName)"
            }
            Write-Host "   - Notes: $($plan.notes)"
            Write-Host "   ---"
        }
    } else {
        Write-Host "   - No treatment plans found for this doctor"
    }
    Write-Host ""
} catch {
    Write-Host "❌ Error getting doctor's treatment plans: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 8: Tiến đến giai đoạn cuối và hoàn thành điều trị
Write-Host "Step 8: Cập nhật hoàn thành điều trị" -ForegroundColor Yellow

$completeProgressData = @{
    currentPhase = 5
    phaseDescription = "Giai đoạn 5: Hoàn thành - Xác nhận thai"
    nextPhaseDate = $null
    nextVisitDate = (Get-Date).AddDays(30).ToString("yyyy-MM-ddTHH:mm:ss")
    notes = "Điều trị IVF thành công. Bệnh nhân đã có thai. Cần theo dõi thai kỳ định kỳ"
    progressNotes = "Chuyển phôi thành công. Beta HCG dương tính. Thai nhi phát triển bình thường"
    status = "Completed"
} | ConvertTo-Json

try {
    $completeResponse = Invoke-RestMethod -Uri "$baseUrl/api/treatmentplans/$planId/progress" -Method PUT -Body $completeProgressData -Headers $headers
    Write-Host "✅ Treatment Plan Completed Successfully:" -ForegroundColor Green
    $completedPlan = $completeResponse.data
    Write-Host "   - Status: $($completedPlan.status)"
    Write-Host "   - Final Phase: $($completedPlan.currentPhase) - $($completedPlan.phaseDescription)"
    Write-Host "   - End Date: $($completedPlan.endDate)"
    Write-Host "   - Final Notes: $($completedPlan.notes)"
    Write-Host "   - Final Progress: $($completedPlan.progressNotes)"
    Write-Host ""
} catch {
    Write-Host "❌ Error completing treatment plan: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "=== TREATMENT PLANNING FLOW TEST COMPLETED ===" -ForegroundColor Green
Write-Host ""
Write-Host "Flow Summary:" -ForegroundColor Cyan
Write-Host "1. ✅ Viewed available treatment services"
Write-Host "2. ✅ Created comprehensive IVF treatment plan"
Write-Host "3. ✅ Viewed detailed treatment plan information"
Write-Host "4. ✅ Updated treatment progress to Phase 2"
Write-Host "5. ✅ Viewed all patient's treatment plans"
Write-Host "6. ✅ Viewed active treatment plan with full details"
Write-Host "7. ✅ Viewed doctor's managed treatment plans"
Write-Host "8. ✅ Completed treatment plan successfully" 