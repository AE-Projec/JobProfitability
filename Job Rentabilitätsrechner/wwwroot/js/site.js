function updateFuelPrice() {
    var fuelType = document.getElementById("fuelType").value;
    var fuelPriceField = document.getElementById("fuelPriceField");
    var fuelPriceLabel = document.getElementById("fuelPriceLabel");
    var fuelInput = document.getElementById("fuelPrice");
    var fuelConsumptionLabel = document.getElementById("fuelConsumptionLabel");
    var transmissionTypeField = document.getElementById("transmissionTypeField");
    var gearCountField = document.getElementById("gearCountField");

    var fuelData = {
        "diesel": { label: "Durchschnittlicher Preis für Diesel (€/Liter):", value: "1.50" },
        "superE10": { label: "Durchschnittlicher Preis für Super E10 (€/Liter):", value: "1.65" },
        "superE5": { label: "Durchschnittlicher Preis für Super E5 (€/Liter):", value: "1.70" },
        "superPlus": { label: "Durchschnittlicher Preis für Super Plus (€/Liter):", value: "1.81" },
        "electricity": { label: "Durchschnittler Preis für Strom (€/kwh):", value: "0.60" } //fuelInput.value ||
    };

    if (fuelType in fuelData) {
        fuelPriceField.style.display = "block";
        fuelPriceLabel.textContent = fuelData[fuelType].label;
        fuelInput.value = fuelData[fuelType].value;

        if (fuelType === "electricity") {
            fuelConsumptionLabel.innerText = "Verbauch (kWh/100km)";
            transmissionTypeField.style.display = "none";
            gearCountField.style.display = "none";
        }
        else {
            fuelConsumptionLabel.innerText = "Verbrauch (Liter/100km)";
            transmissionTypeField.style.display = "block";
        }
    } else {
        fuelPriceField.style.display = "none";
    }
}

document.addEventListener("DOMContentLoaded", function () {

 
    //fuel price update
    var fuelTypeElement = document.getElementById("fuelType");
    if (fuelTypeElement) {
        fuelTypeElement.addEventListener("change", updateFuelPrice);
        updateFuelPrice();
    }
    //soli checkbox handling
    document.getElementById("salaryForm").addEventListener("input", function () {
        var grossSalary = parseFloat(document.getElementById("grossSalary").value || 0);
        var newGrossSalary = parseFloat(document.getElementById("newGrossSalary").value || 0);
        var taxClass = parseInt(document.getElementById("taxClass").value, 10);

        // Grenzen für 2024
        var soliThreshold;
        if (taxClass === 3 || taxClass === 4) {
            soliThreshold = 136826;
        }
        else {
            soliThreshold = 68413;
        }

        // Deaktivieren der Checkbox, wenn das Bruttoeinkommen über der Grenze liegt
        if (grossSalary > soliThreshold || newGrossSalary > soliThreshold) {
            document.getElementById("includeSoli").checked = true;
            document.getElementById("includeSoli").disabled = true;
            document.getElementById("soliContainer").style.display = "block";
        } else {
            document.getElementById("includeSoli").checked = false;
            document.getElementById("includeSoli").disabled = false;
            document.getElementById("soliContainer").style.display = "none";
        }
        
    });

    //handling wear and tear
    var wearLevelContainer = document.getElementById("wearLevelContainer");
    var wearLevelInput = document.getElementById("wearLevel");
    var includeWearAndTearCheckbox = document.getElementById("includeWearAndTear");

    // Initialisierung: Anzeige basierend auf dem Zustand der Checkbox
    if (includeWearAndTearCheckbox.checked) {
        wearLevelContainer.style.display = "block";
        wearLevelInput.disabled = false;
    } else {
        wearLevelContainer.style.display = "none";
        wearLevelInput.disabled = true;
    }

    // Event-Listener für die Checkbox zur Anzeige/Verbergung des Containers
    includeWearAndTearCheckbox.addEventListener("change", function () {
        if (this.checked) {
            wearLevelContainer.style.display = "block";
            wearLevelInput.disabled = false;
        } else {
            wearLevelContainer.style.display = "none";
            wearLevelInput.disabled = true;
        }
    });

    //Gear count visibility
    var transmissionTypeSelect = document.getElementById("transmissionType");
    var gearCountField = document.getElementById("gearCountField");

    function updateGearCountVisibility() {
        if (transmissionTypeSelect.value === "automatic") {
            gearCountField.style.display = "block";
        }
        else {
            gearCountField.style.display = "none";
        }
    }
    transmissionTypeSelect.addEventListener("change", updateGearCountVisibility);
    updateGearCountVisibility();

    //Modal Handling
    //Klicken außerhalb des Modals, um es zu schließen
    var modals = document.getElementsByClassName('modal');
    window.onclick = function (event) {
        for (var i = 0; i < modals.length; i++) {
            if (event.target == modals[i]) {
                modals[i].style.display = "none";
            }
        }
    }

});



document.addEventListener("DOMContentLoaded", function () {
    updateFuelPrice();
});



function calculateNetSalary(type) {
    var grossSalary, resultElement;

    if (type === "old") {
        grossSalary = document.getElementById("grossSalary").value;
        resultElement = document.getElementById("netSalaryNew");
    }
    else if (type === "new") {
        grossSalary = document.getElementById("newGrossSalary").value;
        resultElement = document.getElementById("newNetSalaryNew");
    }
  
    
    var taxClass = document.getElementById("taxClass").value;
    var churchTax = document.getElementById("churchTax").checked;

    fetch(`/Calculate?handler=CalculateNetSalary&grossSalary=${grossSalary}&newGrossSalary=${newGrossSalary}&taxClass=${taxClass}&churchTax=${churchTax}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.text();  //  text() anstelle von json(), um zu sehen, was die Antwort tatsächlich ist
        })
        .then(text => {
            let data;
            try {
                data = JSON.parse(text);  // Versuche, den Text als JSON zu parsen
            } catch (error) {
                console.error('Fehler beim Parsen von JSON:', error, 'Antworttext:', text);
                return;
            }
            if (data.Error) {
                console.error('Server Error:', data.Error);
            } else {
                resultElement.innerText = data.netSalary.toFixed(2);
            }
        })
        .catch(error => console.error('Fehler:', error));
}



document.addEventListener("DOMContentLoaded", function () {
    var bundeslandContainer = document.getElementById("bundeslandContainer");
    var kirchensteuerCheckBox = document.getElementById("churchTax");

    if (kirchensteuerCheckBox.checked) {
        bundeslandContainer.style.display = "block";
    }
    else {
        bundeslandContainer.style.display = "none";
        hideModalOnChurchCheckbox();
    }

    // Event-Listener für die Checkbox zur Anzeige/Verbergung des Containers
    kirchensteuerCheckBox.addEventListener("change", function () {
        if (this.checked) {
            bundeslandContainer.style.display = "block";
        } else {
            bundeslandContainer.style.display = "none";
        }
    });

});

function showInfoModal(modalId) {
    var modal = document.getElementById(modalId);
    if (modal) {
        modal.style.display = "block";
    }
    //document.getElementById("infoModal").style.display = "block";
}

function closeInfoModal(modalId) {
    var modal = document.getElementById(modalId);
    if (modal) {
        modal.style.display = "none";
    }
    //document.getElementById("infoModal").style.display = "none";
}

// Diese Funktion versteckt das Modal, wenn sich die Getriebeart ändert
function hideModalOnGearChange() {
    var modal = document.getElementById('infoModalGearCount');
    if (modal) {
        modal.style.display = "none";
    }
}

function hideModalOnChurchCheckbox() {
    var modal = document.getElementById('infoModalBundesland');
    if (modal) {
        modal.style.display = "none";
        
    }
}
function hideModalOnWearAndTearCheckbox() {
    var modal = document.getElementById('infoWearAndTear');
    if (modal) {
        modal.style.display = "none";

    }
}



function handleChurchCheckbox() {
    var calculatedBruttoWithChurch = document.getElementById("calculatedGrossSalaryWithChurchTaxes");
    var calculatedBruttoWithoutChurch = document.getElementById("calculatedGrossSalaryWithoutChurchTaxes");
    var churchTaxCheckBox = document.getElementById("churchTax");

    if (churchTaxCheckBox.checked) {
        if (calculatedBruttoWithoutChurch) {
            calculatedBruttoWithoutChurch.style.display = "none";
        }
        if (calculatedBruttoWithChurch) {
            calculatedBruttoWithChurch.style.display = "table-row";
        }
    } else {
        if (calculatedBruttoWithChurch) {
            calculatedBruttoWithChurch.style.display = "none";
        }
        if (calculatedBruttoWithoutChurch) {
            calculatedBruttoWithoutChurch.style.display = "table-row";
        }
    }
}
document.addEventListener("DOMContentLoaded", function () {
    handleChurchCheckbox();
});

// Diese Funktion wird aufgerufen, wenn sich die Getriebeart oder Kraftstofftyp ändert
function handleTransmissionOrFuelChange() {
    hideModalOnGearChange();
    // Weitere Logik, um Felder abhängig von der Auswahl ein- oder auszublenden
}



function toggleExternalNettoInput() {
    var useExternalNetto = document.querySelector('input[name="useExternalNetto"]:checked').value;
    var externalNettoInput = document.getElementById("externalNettoInput");
    var oldBruttoInput = document.getElementById("oldBruttoInput");
    var newBruttoInput = document.getElementById("newBruttoInput");
    var grossSalaryWithChurchTaxes = document.getElementById("calculatedGrossSalaryWithChurchTaxes");
    var grossSalaryWithOutChurchTaxes = document.getElementById("calculatedGrossSalaryWithoutChurchTaxes");
    var calculatedNetto = document.getElementById("calculatedNetSalary");
    var taxClass = document.getElementById("taxClassComp");
    var churchTax = document.getElementById("churchTaxComp");

 


    if (useExternalNetto === 'true') {
        externalNettoInput.style.display = "block";
        taxClass.style.display = "none";
        churchTax.style.display = "none";
        oldBruttoInput.style.display = "none";
        newBruttoInput.style.display = "none";
        grossSalaryWithOutChurchTaxes.style.display = "none";
        calculatedNetto.style.display = "none";
    } else {
        externalNettoInput.style.display = "none";
        oldBruttoInput.style.display = "block";
        newBruttoInput.style.display = "block";
        taxClass.style.display = "block";
        churchTax.style.display = "block";
        calculatedNetto.style.display = "table-row";
       
    }
    
}

// Initialisiere die Anzeige beim Laden der Seite
document.addEventListener("DOMContentLoaded", function () {
    toggleExternalNettoInput(); // Zeige/hide Felder abhängig vom initialen Radio-Button-Zustand
});

// Event Listener hinzufügen
document.querySelectorAll('input[name="useExternalNetto"]').forEach(function (radio) {
    radio.addEventListener('change', toggleExternalNettoInput);
});



function clearPlaceholder(input) {
    if (input.value === "0" || input.value === "0.00") {
        input.value = "";
    }
}


// Event Listener 
document.getElementById('transmissionTypeField').addEventListener('change', handleTransmissionOrFuelChange);
document.getElementById('fuelType').addEventListener('change', handleTransmissionOrFuelChange);

document.getElementById('churchTax').addEventListener('change', handleChurchCheckbox);
document.getElementById('bundesland').addEventListener('change', hideModalOnChurchCheckbox);



