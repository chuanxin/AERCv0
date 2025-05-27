// Placeholder data arrays for dropdowns
const unitDDL = [ { value: '', text: '--請選擇--' }, { value: 'DIY', text: 'DIY' }, { value: 'Contractor', text: '委外' }];
const l1QualityDDL = [ { value: '', text: '--請選擇--' }, { value: 'PVC', text: 'PVC管' }, { value: 'PE', text: 'PE管' }, { value: 'Stainless', text: '不鏽鋼管' }];
const l1SpecDDL = [ { value: '', text: '--請選擇--' }, { value: '50', text: '50mm' }, { value: '75', text: '75mm' }, { value: '100', text: '100mm' }];
const l2QualityDDL = [ { value: '', text: '--請選擇--' }, { value: 'PVC_sub', text: 'PVC副管' }, { value: 'PE_sub', text: 'PE副管' }];
const l2SpecDDL = [ { value: '', text: '--請選擇--' }, { value: '20', text: '20mm' }, { value: '25', text: '25mm' }];

const END_TYPE_PLUG = 'plug';
const END_TYPE_VALVE = 'valve';
const IRRIGATION_TYPE_DRIP = 'drip';
const IRRIGATION_TYPE_SPRAY = 'spray';
const IRRIGATION_TYPE_PERFORATED = 'perforated';
const IRRIGATION_TYPE_NONE = 'none';

const endTypeDDL = [
    { value: '', text: '--請選擇--' }, { value: END_TYPE_PLUG, text: '管塞' }, { value: END_TYPE_VALVE, text: '閥門' },
    { value: IRRIGATION_TYPE_DRIP, text: '滴灌系統' }, { value: IRRIGATION_TYPE_SPRAY, text: '噴灌系統' }, { value: IRRIGATION_TYPE_PERFORATED, text: '穿孔管系統' }
];

const NONE_FAC_TYPE_VALUE = 'none_facility';

const dropDDL = [ { value: '', text: '--請選擇--' }, { value: 'drop1', text: '滴灌類型1' }, { value: 'drop2', text: '滴灌類型2' }];
const sprayDDL = [ { value: '', text: '--請選擇--' }, { value: 'spray1', text: '噴灌類型1' }, { value: 'spray2', text: '噴灌類型2' }];
const perforatedDDLData = [ { value: '', text: '--請選擇--' }, { value: 'perf1', text: '穿孔管類型1'}];
const facTypeDDL = [ { value: '', text: '--請選擇--' }, { value: 'typeA', text: '設施類型A' }, { value: 'typeB', text: '設施類型B' }, { value: NONE_FAC_TYPE_VALUE, text: '無設施'}];
const waterSrcDDL = [ { value: '', text: '--請選擇--' }, { value: 'src1', text: '水源1' }, { value: 'src2', text: '水源2' }];
const branchPipeMaterialDDL = [ { value: '', text: '--請選擇--' }, { value: 'mat_branch1', text: '支管材質1' }, { value: 'mat_branch2', text: '支管材質2' }];
const branchPipeSpecDDL = [ { value: '', text: '--請選擇--' }, { value: 'spec_branch1', text: '支管規格1' }, { value: 'spec_branch2', text: '支管規格2' }];
const nozzleSpecDDL = [ { value: '', text: '--請選擇--' }, { value: 'spec_nozzle1', text: '噴頭規格1' }, { value: 'spec_nozzle2', text: '噴頭規格2' }];
const nozzleMaterialDDL = [ { value: '', text: '--請選擇--' }, { value: 'type_nozzle1', text: '噴頭材質1' }, { value: 'type_nozzle2', text: '噴頭材質2' }];
const stdpipeMaterialDDL = [ { value: '', text: '--請選擇--' }, { value: 'mat_stdpipe1', text: '標準管材1' }, { value: 'mat_stdpipe2', text: '標準管材2' }];
const stdpipeSpecDDL = [ { value: '', text: '--請選擇--' }, { value: 'spec_stdpipe1', text: '標準管徑1' }, { value: 'spec_stdpipe2', text: '標準管徑2' }];
const groupDDL = [ 
    { value: '', text: '--請選擇管材組--' }, 
    { value: 'NEW_MAINPIPE', text: '新增主要管材' }, 
    { value: 'NEW_FITTINGS', text: '新增配件' },
    { value: 'NEW_AUX', text: '新增輔助材料'}
];

// --- Helper functions for visibility ---
function toggleVisibility(controlElementValue, targetValueToShow, elementToShowId, elementToHideId) {
    const showElement = document.getElementById(elementToShowId);
    const hideElement = document.getElementById(elementToHideId);
    if (!showElement || !hideElement) { return; }
    if (controlElementValue === targetValueToShow) {
        showElement.style.display = ''; hideElement.style.display = 'none';
    } else {
        showElement.style.display = 'none'; hideElement.style.display = '';
    }
}

function setElementVisibility(elementId, isVisible) {
    const element = document.getElementById(elementId);
    if (element) { element.style.display = isVisible ? '' : 'none'; }
}

// Function to populate a dropdown
function populateDropdown(selectElementId, dataArray) {
    const selectElement = document.getElementById(selectElementId);
    if (selectElement) {
        selectElement.innerHTML = '';
        dataArray.forEach(item => {
            const option = document.createElement('option');
            option.value = item.value; option.textContent = item.text;
            selectElement.appendChild(option);
        });
    } else { console.warn(`Dropdown element with ID '${selectElementId}' not found.`); }
}

function initializeFormDropdowns() {
    populateDropdown('ddl_FarmerSysUnit', unitDDL);
    // Add required to ddl_FarmerSysUnit after populating if needed
    // const ddlFarmerSysUnit = document.getElementById('ddl_FarmerSysUnit');
    // if (ddlFarmerSysUnit) ddlFarmerSysUnit.required = true;


    populateDropdown('L1Mat', l1QualityDDL); populateDropdown('L1Spec', l1SpecDDL);
    populateDropdown('ddl_EndType', endTypeDDL); populateDropdown('ddl_Group', groupDDL); // ddl_Group is now populated
    populateDropdown('ddl_Drop', dropDDL); populateDropdown('ddl_Sprinkler', sprayDDL);
    populateDropdown('ddl_Perforated', perforatedDDLData);
    populateDropdown('ddl_FacType', facTypeDDL);
}

// --- Material Data Definitions ---
// These will be filled by the restoration logic at the end of the script
const initialMaterialsData = []; 
const standardMaterialsData = [];


// --- Material Table Rendering and Population ---
function renderMaterialRowHtml(item, groupName) { // groupName is not used by current render logic but kept for potential future use
    return `
        <tr id="tr_p_${item.matGroup}_${item.pomNo}">
            <td style="display:none;">${item.pomNo}</td> <td>${item.matOrderCNS}</td> <td>${item.mName}</td>
            <td>${item.moduleCNS}</td> <td>${item.spec}</td> <td>${item.itemUnit}</td> <td>${item.note}</td>
            <td><input type="number" step="any" min="0" value="${item.price}" class="mat_price input-small" onchange="CalTotal(this)"></td>
            <td><input type="number" step="any" min="0" value="${item.amount}" class="mat_num input-small" onchange="CalTotal(this)"></td>
            <td><input type="text" value="${item.totalPrice}" class="mat_total input-small" readonly="readonly"></td>
            <td style="display:none;">${item.matOrder}</td>
            <td>
                <button type="button" onclick="UpMatOrder(this)">↑</button> <button type="button" onclick="DownMatOrder(this)">↓</button> <button type="button" onclick="DelMat(this)">刪除</button>
            </td>
        </tr>`;
}

function populateInitialMaterials(data) {
    const materialTableBody = document.querySelector("#MatTabl_MainPipe tbody");
    if (!materialTableBody) { return; }
    materialTableBody.innerHTML = ''; 
    if (data.length === 0) {
         materialTableBody.innerHTML = '<tr><td colspan="10" style="text-align:center;">(無資料)</td></tr>';
         return;
    }
    data.forEach(groupitem => {
        materialTableBody.innerHTML += `<tr class="material-group-header"><td colspan="10" style="background-color:#f0f0f0; font-weight:bold;">${groupitem.groupName}</td></tr>`;
        groupitem.list.forEach(item => {
            materialTableBody.innerHTML += renderMaterialRowHtml(item, groupitem.groupName);
        });
    });
}

// --- Calculation Functions ---
// Full versions of these functions are at the end of the script due to previous edits.
// The script execution should use those. These are simplified placeholders from earlier stages.
function calculateBlockArea() { /* Placeholder, full version at end */ }
function CalTotal(element) { /* Placeholder, full version at end */ }
function updateGrandTotal() { /* Placeholder, full version at end */ }

// --- Material Row Action Functions ---
function UpMatOrder(element) {
    const row = element.closest('tr');
    if (!row) return;
    const previousRow = row.previousElementSibling;
    // Check if previous row exists and is not a group header
    if (previousRow && !previousRow.classList.contains('material-group-header')) { 
        row.parentNode.insertBefore(row, previousRow);
    }
}

function DownMatOrder(element) {
    const row = element.closest('tr');
    if (!row) return;
    const nextRow = row.nextElementSibling;
    if (nextRow) { // Standard next sibling check
        // To prevent moving past the next group's header, check if nextRow is a group header
        if (nextRow.classList.contains('material-group-header')) return;

        // If the next sibling of `nextRow` is the one to insert before.
        // If `nextRow` is the last element, `nextRow.nextElementSibling` will be null, 
        // and `insertBefore` with null second arg appends to the end of the list of children.
        row.parentNode.insertBefore(row, nextRow.nextElementSibling);
    }
}

function DelMat(element) {
    // Full version of this function is at the end of the script.
    // This is a simplified placeholder from earlier stages.
    console.log('DelMat called for element:', element);
    const row = element.closest('tr');
    if (row) {
        row.remove();
        updateGrandTotal(); // This will call the full updateGrandTotal
    }
}


// --- Event Listeners for Conditional Visibility ---
// Full versions of these functions are at the end of the script.
function handleEndTypeChange() { /* Placeholder, full version at end */ }
function handleFacTypeChange() { /* Placeholder, full version at end */ }

// --- Function to Load Standard Materials ---
// Full version of this function is at the end of the script.
function loadStandardMaterials() { /* Placeholder, full version at end */ }

// --- Add New Material Logic ---
function handleOpenMaterialPopup() {
    const divGroup = document.getElementById('div_Group');
    if (divGroup) {
        divGroup.style.display = 'block'; 
    }
}

function handleCloseGroupPopup() {
    const divGroup = document.getElementById('div_Group');
    if (divGroup) {
        divGroup.style.display = 'none';
    }
}

function handleSendGroupSelection() {
    const ddlGroup = document.getElementById('ddl_Group');
    const materialTableBody = document.querySelector("#MatTabl_MainPipe tbody");

    if (!ddlGroup || !materialTableBody) {
        console.error("Required elements for adding material not found.");
        return;
    }
    const selectedGroupValue = ddlGroup.value;
    const selectedGroupText = ddlGroup.options[ddlGroup.selectedIndex].text;
    if (!selectedGroupValue) {
        alert("請選擇一個管材組！");
        return;
    }
    const noDataRow = materialTableBody.querySelector('td[colspan="10"]');
    if (noDataRow && noDataRow.textContent.includes("(無資料)")) {
        const parentRow = noDataRow.closest('tr');
        if (parentRow) parentRow.remove();
    }
    const newItem = {
        pomNo: `NEW_${selectedGroupValue}_${Date.now()}`, matGroup: selectedGroupValue, matOrder: 99, 
        matOrderCNS: "新項目", mName: "新物料 (點擊修改)", moduleCNS: selectedGroupText, 
        spec: "N/A", itemUnit: "個", note: "手動新增", price: 0, amount: 1, totalPrice: 0 
    };
    let groupHeaderFound = false;
    const groupHeaders = materialTableBody.querySelectorAll('.material-group-header');
    groupHeaders.forEach(header => {
        if (header.textContent.trim() === selectedGroupText.trim()) {
            groupHeaderFound = true;
        }
    });
    let newRowHtml = '';
    if (!groupHeaderFound) {
        newRowHtml += `<tr class="material-group-header"><td colspan="10" style="background-color:#f0f0f0; font-weight:bold;">${selectedGroupText}</td></tr>`;
    }
    newRowHtml += renderMaterialRowHtml(newItem, selectedGroupText);
    materialTableBody.insertAdjacentHTML('beforeend', newRowHtml);
    updateGrandTotal(); // This will call the full updateGrandTotal
    handleCloseGroupPopup();
}


// --- DOMContentLoaded Event Listener ---
document.addEventListener('DOMContentLoaded', function() {
    initializeFormDropdowns();
    // The initialMaterialsData and standardMaterialsData arrays are empty here,
    // but they are filled by the restoration logic further down before being used.
    populateInitialMaterials(initialMaterialsData); 
    updateGrandTotal();  // This will call the full updateGrandTotal
    calculateBlockArea(); // This will call the full calculateBlockArea

    setElementVisibility('ddl_Drop', false);
    setElementVisibility('ddl_Sprinkler', false);
    setElementVisibility('ddl_Perforated', false);

    const lengthInput = document.getElementById('Length');
    if (lengthInput) lengthInput.addEventListener('input', calculateBlockArea);
    const widthInput = document.getElementById('width');
    if (widthInput) widthInput.addEventListener('input', calculateBlockArea);

    const ddlEndType = document.getElementById('ddl_EndType');
    if (ddlEndType) ddlEndType.addEventListener('change', handleEndTypeChange);
    const ddlFacType = document.getElementById('ddl_FacType');
    if (ddlFacType) ddlFacType.addEventListener('change', handleFacTypeChange);
    
    const btnLoadStd = document.getElementById('btn_LoadStd');
    if (btnLoadStd) btnLoadStd.addEventListener('click', loadStandardMaterials);

    const btnOpenMaterialPopup = document.getElementById('btn_OpenMaterialPopup');
    if (btnOpenMaterialPopup) btnOpenMaterialPopup.addEventListener('click', handleOpenMaterialPopup);

    const btnCloseGroupPopup = document.getElementById('btn_CloseGroupPopup');
    if (btnCloseGroupPopup) btnCloseGroupPopup.addEventListener('click', handleCloseGroupPopup);

    const btnSendGroupSelection = document.getElementById('btn_SendGroupSelection');
    if (btnSendGroupSelection) btnSendGroupSelection.addEventListener('click', handleSendGroupSelection);
    
    const ddlGroup = document.getElementById('ddl_Group');
    if(ddlGroup) ddlGroup.addEventListener('change', function() {
        console.log("Selected group for new material:", this.value, this.options[this.selectedIndex].text);
    });

    if (ddlEndType) ddlEndType.dispatchEvent(new Event('change'));
    if (ddlFacType) ddlFacType.dispatchEvent(new Event('change'));
});

// Restore existing data for initialMaterialsData and standardMaterialsData
const initialMaterialsData_actual = [
    {
        groupName: "主要管材 (預設)",
        list: [
            { pomNo: 'P001', matGroup: 'MAINPIPE', matOrder: 1, matOrderCNS: '田間主管1', mName: 'PVC硬管', moduleCNS: '管材', spec: '4英寸 SCH80', itemUnit: '米', note: '高壓主幹', price: 120, amount: 50, totalPrice: 6000 },
            { pomNo: 'P002', matGroup: 'MAINPIPE', matOrder: 2, matOrderCNS: '田間主管2', mName: 'PE軟管', moduleCNS: '管材', spec: '2英寸 SDR11', itemUnit: '米', note: '低壓分支', price: 30, amount: 100, totalPrice: 3000 }
        ]
    },
    {
        groupName: "配件 (預設)",
        list: [
            { pomNo: 'F001', matGroup: 'FITTINGS', matOrder: 1, matOrderCNS: '接頭', mName: 'PVC彎頭', moduleCNS: '配件', spec: '4英寸 90度', itemUnit: '個', note: '', price: 50, amount: 10, totalPrice: 500 }
        ]
    }
];
initialMaterialsData.length = 0; 
initialMaterialsData.push(...initialMaterialsData_actual);


const standardMaterialsData_actual = [
    {
        groupName: "標準自動帶入管材",
        list: [
            { pomNo: 'STD_P001', matGroup: 'STDPIPE', matOrder: 1, matOrderCNS: '標準主管', mName: '耐壓PE管', moduleCNS: '管材', spec: '3英寸 PN10', itemUnit: '卷 (100米)', note: '自動帶入標準品', price: 800, amount: 1, totalPrice: 800 },
            { pomNo: 'STD_F001', matGroup: 'STDFIT', matOrder: 1, matOrderCNS: '標準接頭', mName: 'PE快速接頭', moduleCNS: '配件', spec: '3英寸', itemUnit: '個', note: '自動帶入標準品', price: 75, amount: 10, totalPrice: 750 },
            { pomNo: 'STD_V001', matGroup: 'STDVALVE', matOrder: 2, matOrderCNS: '標準閥門', mName: '塑膠球閥', moduleCNS: '閥件', spec: '3英寸', itemUnit: '個', note: '自動帶入標準品', price: 150, amount: 2, totalPrice: 300 }
        ]
    },
    {
        groupName: "標準輔助材料",
        list: [
            { pomNo: 'STD_A001', matGroup: 'STDAUX', matOrder: 1, matOrderCNS: '止洩帶', mName: 'PTFE止洩帶', moduleCNS: '輔材', spec: '標準寬度', itemUnit: '卷', note: '自動帶入', price: 10, amount: 5, totalPrice: 50 }
        ]
    }
];
standardMaterialsData.length = 0; 
standardMaterialsData.push(...standardMaterialsData_actual);

// Full versions of core functions (ensure these are the ones used)
function CalTotal(element) {
    const row = element.closest('tr'); if (!row) return;
    const priceInput = row.querySelector('.mat_price'); const amountInput = row.querySelector('.mat_num');
    const totalInput = row.querySelector('.mat_total');
    if (!priceInput || !amountInput || !totalInput) { return; }
    const price = parseFloat(priceInput.value) || 0; const amount = parseFloat(amountInput.value) || 0;
    totalInput.value = (price * amount).toFixed(2);
    updateGrandTotal();
}

function updateGrandTotal() {
    let grandTotal = 0;
    const rows = document.querySelectorAll('#MatTabl_MainPipe tbody tr:not(.material-group-header)');
    rows.forEach(row => {
        const totalInput = row.querySelector('.mat_total');
        if (totalInput) { grandTotal += parseFloat(totalInput.value) || 0; }
    });
    const grandTotalInput = document.getElementById('txt_Mat_Total');
    if (grandTotalInput) { grandTotalInput.value = grandTotal.toFixed(2); }
}

function DelMat(element) { // This is the DelMat that should be used by the onclick handlers
    console.log('DelMat called (full version) for element:', element);
    const row = element.closest('tr');
    if (row) {
        row.remove();
        updateGrandTotal(); 

        const materialTableBody = document.querySelector("#MatTabl_MainPipe tbody");
        if (materialTableBody) {
            let onlyHeaders = true;
            if (materialTableBody.children.length === 0) {
                onlyHeaders = false; 
            } else {
                for (let child of materialTableBody.children) {
                    if (!child.classList.contains('material-group-header')) {
                        onlyHeaders = false;
                        break;
                    }
                }
            }
            if (materialTableBody.children.length === 0 || onlyHeaders) {
                materialTableBody.innerHTML = '<tr><td colspan="10" style="text-align:center;">(無資料)</td></tr>';
            }
        }
    } else {
        console.warn('DelMat: could not find parent row for element', element);
    }
}

function handleEndTypeChange() {
    const selectedValue = this.value;
    setElementVisibility('ddl_Drop', selectedValue === IRRIGATION_TYPE_DRIP);
    setElementVisibility('ddl_Sprinkler', selectedValue === IRRIGATION_TYPE_SPRAY);
    setElementVisibility('ddl_Perforated', selectedValue === IRRIGATION_TYPE_PERFORATED);
    const usesBranchPipes = selectedValue !== END_TYPE_PLUG && selectedValue !== IRRIGATION_TYPE_NONE;
    setElementVisibility('div_BranchPipeMaterialTrue', usesBranchPipes); setElementVisibility('div_BranchPipeMaterialFalse', !usesBranchPipes);
    setElementVisibility('div_BranchPipeSpecTrue', usesBranchPipes); setElementVisibility('div_BranchPipeSpecFalse', !usesBranchPipes);
    setElementVisibility('div_SSTrue', usesBranchPipes); setElementVisibility('div_SSFalse', !usesBranchPipes);
    const usesStandpipes = selectedValue === END_TYPE_VALVE || selectedValue === IRRIGATION_TYPE_SPRAY;
    setElementVisibility('div_PipeHeightTrue', usesStandpipes); setElementVisibility('div_PipeHeightFalse', !usesStandpipes);
    setElementVisibility('div_PipeMaterialTrue', usesStandpipes); setElementVisibility('div_PipeMaterialFalse', !usesStandpipes);
    setElementVisibility('div_PipeSpecTrue', usesStandpipes); setElementVisibility('div_PipeSpecFalse', !usesStandpipes);
}

function handleFacTypeChange() {
    toggleVisibility(this.value, NONE_FAC_TYPE_VALUE, 'div_FacTypeFalse', 'div_FacTypeTrue');
}

function loadStandardMaterials() {
    console.log("Loading standard materials...");
    populateInitialMaterials(standardMaterialsData);
    updateGrandTotal(); 
}

function calculateBlockArea() {
    const lengthInput = document.getElementById('Length'); const widthInput = document.getElementById('width');
    const buildAreaInput = document.getElementById('BuildArea');
    if(!lengthInput || !widthInput || !buildAreaInput) return;
    const length = parseFloat(lengthInput.value) || 0; const width = parseFloat(widthInput.value) || 0;
    const area = length * width;
    buildAreaInput.value = isNaN(area) ? '' : area.toFixed(2);
}
