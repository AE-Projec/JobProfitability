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
        "electricity": { label: "Durchschnittler Preis für Strom (€/kwh):", value: "0.60" }
    };

    if (fuelType in fuelData) {
        fuelPriceField.style.display = "block";
        fuelPriceLabel.textContent = fuelData[fuelType].label;
        fuelInput.value = fuelData[fuelType].value;

        if (fuelType === "electricity") {
            fuelConsumptionLabel.innerText = "Verbauch (kWh/100km):";
            transmissionTypeField.style.display = "none";
            gearCountField.style.display = "none";
        }
        else {
            fuelConsumptionLabel.innerText = "Verbrauch (Liter/100km):";
            transmissionTypeField.style.display = "block";
        }
    } else {
        fuelPriceField.style.display = "none";
    }
}

document.querySelectorAll('.card-header').forEach(function (header) {
    header.addEventListener('click', function () {
        const button = header.querySelector('.btn');
        if (button) {
            button.click();
        }
    });
});


function checkSoli() {
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

    // aktivieren der Checkbox, wenn das Bruttoeinkommen über der Grenze liegt
    if (grossSalary > soliThreshold || newGrossSalary > soliThreshold) {
        document.getElementById("includeSoli").checked = true;
        document.getElementById("includeSoli").disabled = true;
        document.getElementById("soliContainer").style.display = "block";
    } else {
        document.getElementById("includeSoli").checked = false;
        document.getElementById("includeSoli").disabled = false;
        document.getElementById("soliContainer").style.display = "none";
    }

}




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
            return response.text();
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


function showInfoModal(modalId) {
    var modal = document.getElementById(modalId);
    if (modal) {
        modal.style.display = "block";
    }
}

function closeInfoModal(modalId) {
    var modal = document.getElementById(modalId);
    if (modal) {
        modal.style.display = "none";
    }
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



// Funktion, um den Erklärungstext basierend auf der Checkbox anzuzeigen oder zu verbergen
document.getElementById("isSachsenCheckbox").addEventListener("change", function () {
    var explanation = document.getElementById("sachsenExplanation");
    if (this.checked) {
        explanation.style.display = "block"; // Zeige den Text an
    } else {
        explanation.style.display = "none"; // Verstecke den Text
    }
});


function handleChurchCheckbox() {
    var calculatedBruttoWithChurch = document.getElementById("calculatedGrossSalaryWithChurchTaxes");
    var calculatedBruttoWithoutChurch = document.getElementById("calculatedGrossSalaryWithoutChurchTaxes");
    var churchTaxCheckBox = document.getElementById("churchTax");
    var useExternalNettoTrue = document.querySelector('input[name="useExternalNetto"]:checked').value === 'true';

    if (useExternalNettoTrue) {
        // Wenn externes Netto verwendet wird, Kirchensteuer ignorieren
        calculatedBruttoWithChurch.style.display = "none"; // Zeile mit Kirchensteuer ausblenden
        calculatedBruttoWithoutChurch.style.display = "table-row"; // Zeile ohne Kirchensteuer anzeigen
        churchTaxCheckBox.checked = false; // Checkbox abwählen
        churchTaxCheckBox.disabled = true; // Checkbox deaktivieren

    } else {
        // Wenn externes Netto nicht verwendet wird, kann die Kirchensteuer berücksichtigt werden
        churchTaxCheckBox.disabled = false; // Checkbox wieder aktivieren
        if (churchTaxCheckBox.checked) {
            // Zeige Gehalt mit Kirchensteuer und blende das ohne Kirchensteuer aus
            calculatedBruttoWithChurch.style.display = "table-row";
            calculatedBruttoWithoutChurch.style.display = "none";
        } else {
            // Zeige Gehalt ohne Kirchensteuer
            calculatedBruttoWithChurch.style.display = "none";
            calculatedBruttoWithoutChurch.style.display = "table-row";
        }
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const calculateYes = document.getElementById("calculateYes");
    const calculateNo = document.getElementById("calculateNo");
    const distanceFields = document.getElementById("distanceFields");
    var oldDistanceFields = document.getElementById("oldDistanceFields");
    var fromLocation = document.getElementById("fromLocation");
    var toLocation = document.getElementById("toLocation");
    const commuteAutomatic = document.getElementById("commuteAutomatic");
    var commuteManual = document.getElementById("commuteManual");
    var oldCommuteManual = document.getElementById("oldCommuteManual");
    var commuteDistanceManual = document.getElementById("commuteDistanceManual");
    var oldCommuteDistanceManual = document.getElementById("oldCommuteDistanceManual");
    var commuteDurationAutomatically = document.getElementById("commuteDurationAutomatically");
    var oldCommuteDuration = document.getElementById("oldCommuteDurationManual");
    var commuteDistance = document.getElementById("commuteDistance");
    var averageCommuteDays = document.getElementById("averageCommuteDays");
    var oldRouteLabel = document.getElementById("oldRouteLabel");
    var newRouteLabel = document.getElementById("newRouteLabel");
    var oldCommuteAutomatic = document.getElementById("oldCommuteAutomatic");
    var oldCommuteDurationAutomatically = document.getElementById("oldCommuteDurationAutomatically");
    var oldCommuteDistance = document.getElementById("oldCommuteDistance");
    var roundTripDistance = document.getElementById("roundTripDistance");
    var oldRoundTripDistance = document.getElementById("oldRoundTripDistance");
    var oldRoute = document.getElementById("oldRoute");
    var newRoute = document.getElementById("newRoute");
    var bundeslandContainer = document.getElementById("bundeslandContainer");
    var kirchensteuerCheckBox = document.getElementById("churchTax");
    var oldAverageCommuteDays = document.getElementById("oldAverageCommuteDays");
    var oldManuallyCommuteDuration = document.getElementById("oldManuallyCommuteDuration");
    var oldCommuteDistanceManualLabel = document.getElementById("oldCommuteDistanceManualLabel");
    var commuteDistanceManualLabel = document.getElementById("commuteDistanceManualLabel");
    var commuteDuration = document.getElementById("commuteDuration");
    var commuteAutomaticDistanceLabel = document.getElementById("commuteAutomaticDistanceLabel");
    var roundTripDistance = document.getElementById("roundTripDistance");
    var roundTripDistanceText = document.getElementById("roundTripDistanceText");
    var calculateOldYes = document.getElementById("calculateOldYes");
    var calculateOldNo = document.getElementById("calculateOldNo");
    var oldCalculateAutomatically = document.getElementById("oldCalculateAutomatically");
    var transmissionTypeField = document.getElementById("transmissionTypeField");
    var gearCountField = document.getElementById("gearCountField");
    var infoModalGearCount = document.getElementById("infoModalGearCount");
    var fuelConsumptionAll = document.getElementById("fuelConsumptionAll");
    var fuelPriceField = document.getElementById("fuelPriceField");
    var personalInformationWithWearAndTear = document.getElementById("personalInformationWithWearAndTear");
    var wearLevelContainer = document.getElementById("wearLevelContainer");
    var infoModalWearAndTear = document.getElementById("infoModalWearAndTear");
    var includeWearAndTearContainer = document.getElementById("includeWearAndTearContainer");
    var commutingCostsAndDistance = document.getElementById("commutingCostsAndDistance");
    var commutingOldCostsAndDistance = document.getElementById("commutingOldCostsAndDistance");

    
    //mobilework
    var mobileWorkYes = document.getElementById("mobileWorkYes");
    var mobileWorkNo = document.getElementById("mobileWorkNo");
    var oldMobileWorkYes = document.getElementById("oldMobileWorkYes");
    var oldMobileWorkNo = document.getElementById("oldMobileWorkNo");
    var commuteDaysPerWeek = document.getElementById("commuteDaysPerWeek");
    var oldCommuteDaysPerWeek = document.getElementById("oldCommuteDaysPerWeek");
    var calculateAutomatically = document.getElementById("calculateAutomatically");
    var commuteCalculations = document.getElementById("commuteCalculations");
    var fuelAndTransmission = document.getElementById("fuelAndTransmission");
    var includeWearAndTeargroup = document.getElementById("includeWearAndTeargroup");
   
    var personalAndWearTear = document.querySelector('h4.mt-4')


    // Funktion, um die Felder anzuzeigen oder auszublenden automatisch berechnen



    function toggleDistanceFields() {
        // Wenn automatische Berechnung aktiv ist
        if (calculateOldYes.checked) {
            oldCommuteDistanceManual.style.display = "none";
            oldCommuteManual.style.display = "none";
            oldManuallyCommuteDuration.style.display = "none";
            oldCommuteDistanceManualLabel.style.display = "none";
            oldDistanceFields.style.display = "block";          
            // Setze manuelle Felder zurück
            oldCommuteDistanceManual.value = '';
            oldCommuteDistance.disabled = true;
            commutingOldCostsAndDistance.textContent = "Alte Automatische Pendelstreckenberechnung";
            //wenn gleichzeitig alte stelle mobilwork ist
            if (oldMobileWorkYes.checked) {
                oldCommuteAutomatic.style.display = "none";
                oldRoute.style.display = "none";
                oldRouteLabel.style.display = "none";
                oldCalculateAutomatically.style.display = " none";
                oldCommuteDistance.value = "";
                oldRoundTripDistance.innerText = "0 Kilometer"
                calculateOldNo.checked = true;
                commutingOldCostsAndDistance.style.display = "none";
                
                
            } else {
                oldCommuteAutomatic.style.display = "block";
                oldRoute.style.display = "block";
                oldRouteLabel.style.display = "block";
                oldCalculateAutomatically.style.display = "block";
                commutingOldCostsAndDistance.style.display = "block";
                
                
            }
        }else {
            oldCommuteManual.style.display = "block";
            oldDistanceFields.style.display = "none";
            oldCommuteDistanceManual.style.display = "block";
            oldManuallyCommuteDuration.style.display = "block";
            oldCommuteDistanceManualLabel.style.display = "block";
            oldCommuteDurationAutomatically.innerText = 'Durchschnittliche Fahrdauer: 0 Minuten';
            oldCommuteDistance.value = '';
            oldRoundTripDistance.innerText = '0 Kilometer';
            oldRouteLabel.style.display = "none";
            oldCommuteAutomatic.style.display = "none";
            oldRoute.style.display = "none";
            commutingOldCostsAndDistance.textContent = "Alte Manuelle Pendelstreckenberechnung";
        }

        if (calculateYes.checked) {
            distanceFields.style.display = "block";
            commuteAutomatic.style.display = "block";
            commuteManual.style.display = "none";
            commuteDistanceManual.style.display = "none";
            commuteDistanceManualLabel.style.display = "none";
            commuteDuration.style.display = "none";
            // Setze manuelle Felder zurück
            commuteDistanceManual.value = '';
            // Deaktiviere die automatischen Pendeldistanz-Felder
            commuteDistance.disabled = true;
            newRoute.style.display = "block";
            newRouteLabel.style.display = "block";
            commuteAutomatic.style.display = "block";
            commutingCostsAndDistance.textContent = "Neue Automatische Pendelstreckenberechnung";

            if (mobileWorkYes.checked) {
                commuteAutomatic.style.display = "none";
                newRoute.style.display = "none";
                newRouteLabel.style.display = "none";
                calculateAutomatically.style.display = " none";
                commuteDistance.value = "";
                roundTripDistance.innerText = "0 Kilometer"
                calculateNo.checked = true;
                commutingCostsAndDistance.style.display = "none";
            }
            else {
                commuteAutomatic.style.display = "block";
                newRoute.style.display = "block";
                newRouteLabel.style.display = "block";
                calculateAutomatically.style.display = "block";
                commutingCostsAndDistance.style.display = "block";
                

            }

            // Steuere die Sichtbarkeit der neuen Route basierend auf neuen mobilen Arbeitsbedingungen
        } else {
            // Manuelle Berechnung aktiv
            distanceFields.style.display = "none";
            commuteDistanceManual.style.display = "block";
            commuteAutomatic.style.display = "none";
            commuteManual.style.display = "block";
            commuteDistanceManualLabel.style.display = "block";
            commuteDuration.style.display = "block";

            // Reset für die automatischen Felder, wenn manuell ausgewählt
            commuteDurationAutomatically.innerText = 'Durchschnittliche Fahrdauer: 0 Minuten';
            commuteDistance.value = '';
            fromLocation.value = '';
            toLocation.value = '';
            roundTripDistance.innerText = '0 Kilometer';
            newRouteLabel.style.display = "none";
            newRoute.style.display = "none";
            commutingCostsAndDistance.textContent = "Neue Manuelle Pendelstreckenberechnung";
        }

        // Steuerung der mobilen Felder unabhängig von der Berechnungsmethode
        if (oldMobileWorkYes.checked) {
            oldCommuteDaysPerWeek.style.display = "none";
            oldCommuteManual.style.display = "none";
            oldAverageCommuteDays.value = "";
            oldCommuteDistance.value = "";
            oldCommuteDistanceManual.value = '';
            oldCalculateAutomatically.style.display = "none";
            commutingOldCostsAndDistance.style.display = "none";
        } else {
            oldCommuteDaysPerWeek.style.display = "block";
            oldCommuteManual.style.display = "block";
            oldCalculateAutomatically.style.display = "block";
            commutingOldCostsAndDistance.style.display = "block";
        }

        if (mobileWorkYes.checked) {
            newRoute.style.display = "none";
            newRouteLabel.style.display = "none";
            commuteAutomatic.style.display = "none";
            calculateAutomatically.style.display = "none";
            commuteDaysPerWeek.style.display = "none";
            commuteManual.style.display = "none";
            commuteDistanceManual.value = '';
            averageCommuteDays.value = "";
            commuteDurationAutomatically.innerText = '0 Minuten';
            commutingCostsAndDistance.style.display = "none";
        } else {
            commuteDaysPerWeek.style.display = "block";
            commuteManual.style.display = "block";
            calculateAutomatically.style.display = "block";
            commutingCostsAndDistance.style.display = "block";
            
        }
        
        if (mobileWorkYes.checked && oldMobileWorkYes.checked) {
            fuelAndTransmission.style.display = "none";
            transmissionTypeField.style.display = "none";
            gearCountField.style.display = "none";
            infoModalGearCount.style.display = "none";
            fuelConsumptionAll.style.display = "none";
            fuelPriceField.style.display = "none";
            includeWearAndTeargroup.style.display = "none";
            wearLevelContainer.style.display = "none";
            infoModalWearAndTear.style.display = "none";
            personalInformationWithWearAndTear.innerText = "Persönliche Angaben"
        }
        else {
            fuelAndTransmission.style.display = "block";
            transmissionTypeField.style.display = "block";
            fuelConsumptionAll.style.display = "block";
            fuelPriceField.style.display = "block";
            includeWearAndTearContainer.style.display = "block";
            personalInformationWithWearAndTear.innerText = "Persönliche Angaben und Fahrzeugabnutzung"
        }
        


        calculateYes.addEventListener("change", toggleDistanceFields);
        calculateNo.addEventListener("change", toggleDistanceFields);

        oldMobileWorkYes.addEventListener("change", toggleDistanceFields);
        oldMobileWorkNo.addEventListener("change", toggleDistanceFields);

        mobileWorkYes.addEventListener("change", toggleDistanceFields);
        mobileWorkNo.addEventListener("change", toggleDistanceFields);

        calculateOldYes.addEventListener("change", toggleDistanceFields);
        calculateOldNo.addEventListener("change", toggleDistanceFields);
        

        document.getElementById("commuteDistanceManual").addEventListener("input", updateCommuteDuration);
        document.getElementById("oldCommuteDistanceManual").addEventListener("input", updateOldCommuteDuration);
    }

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

    //fuel price update
    var fuelTypeElement = document.getElementById("fuelType");
    if (fuelTypeElement) {
        fuelTypeElement.addEventListener("change", updateFuelPrice);
        updateFuelPrice();
    }
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


    checkSoli();
    updateFuelPrice();


    // Event Listener 
    document.getElementById('transmissionTypeField').addEventListener('change', handleTransmissionOrFuelChange);
    document.getElementById('fuelType').addEventListener('change', handleTransmissionOrFuelChange);


    document.getElementById('churchTax').addEventListener('change', handleChurchCheckbox);
    document.getElementById('bundesland').addEventListener('change', hideModalOnChurchCheckbox);

    // Initialer Aufruf zum Setzen des Zustands basierend auf dem aktuellen Radio-Button-Wert
    toggleDistanceFields();
    toggleMobileWorkFields();
    toggleOldMobileWorkFields();

    handleChurchCheckbox();
    hideModalOnChurchCheckbox();

    toggleExternalNettoInput(); // Zeige/hide Felder abhängig vom initialen Radio-Button-Zustand



});


// Event Listener hinzufügen
document.querySelectorAll('input[name="useExternalNetto"]').forEach(function (radio) {
    radio.addEventListener('change', toggleExternalNettoInput);
});

//event listener für radios
document.getElementById("falseRadio").addEventListener("change", toggleExternalNettoInput);
document.getElementById("truthRadio").addEventListener("change", toggleExternalNettoInput);




document.addEventListener("DOMContentLoaded", function () {
    const grossSalaryField = document.getElementById("grossSalary");
    const validationMessage = document.getElementById("validationMessageOldGross");
    const externalNettoCheckbox = document.getElementById("externalNettoCheckbox");

    // Validierungsfunktion für das Gehaltsfeld
    function validateGrossSalary() {
        const minValue = 10000;
        if (grossSalaryField.value < minValue) {
            validationMessage.style.display = 'block';
            grossSalaryField.setCustomValidity(`Das Gehalt muss mindestens ${minValue} Euro betragen.`);
        } else {
            grossSalaryField.setCustomValidity('');
            validationMessage.style.display = 'none';
        }
    }

    // Überprüfe Sichtbarkeit und initialisiere den Listener nur bei Bedarf
    if (!externalNettoCheckbox.checked) {
        grossSalaryField.addEventListener('input', validateGrossSalary);
    }

    // Event-Listener, um das Gehaltsfeld bei Bedarf anzuzeigen und Listener zu verwalten
    externalNettoCheckbox.addEventListener("change", function () {
        if (externalNettoCheckbox.checked) {
            validationMessage.style.display = 'none';
            grossSalaryField.removeEventListener('input', validateGrossSalary);
        } else {
            grossSalaryField.addEventListener('input', validateGrossSalary);
        }
    });
});

document.getElementById("salaryForm").addEventListener("input", function () {
    checkSoli();
});



function convertMinutesToTime(minutes) {
    var wholeMinutes = Math.floor(minutes);  // Ganze Minuten
    var seconds = Math.round((minutes - wholeMinutes) * 60);  // Dezimal in Sekunden umwandeln
    return `${wholeMinutes} Minuten und ${seconds} Sekunden`;
}

function updateOldCommuteDuration() {
    var oldCommuteDistance = parseFloat(document.getElementById("oldCommuteDistanceManual").value);
    var oldCommuteDuration = document.getElementById("oldCommuteDurationManual");
    var oldHiddenCommuteDuration = document.getElementById("hiddenOldCommuteDuration");

    if (oldCommuteDistance && oldCommuteDistance > 0) {
        var oldEstimatedDuration = (oldCommuteDistance / 50) * 60;  // Beispiel: 50 km/h Durchschnittsgeschwindigkeit
        var oldFormattedTime = convertMinutesToTime(oldEstimatedDuration);
        oldCommuteDuration.innerText = `Durchschnittliche Fahrtdauer (basierend auf 50 km/h): ${oldFormattedTime}`;

        oldHiddenCommuteDuration.value = oldEstimatedDuration.toFixed(2);

    } else {
        oldCommuteDuration.innerText = `Durchschnittliche Fahrtdauer (basierend auf 50 km/h): 0 Minuten`;

    }
}


function updateCommuteDuration() {
    var commuteDistance = parseFloat(document.getElementById("commuteDistanceManual").value);
    var commuteDuration = document.getElementById("commuteDurationManual");
    var hiddenCommuteDuration = document.getElementById("hiddenCommuteDuration");

    if (commuteDistance && commuteDistance > 0) {
        var estimatedDuration = (commuteDistance / 50) * 60;  // Beispiel: 50 km/h Durchschnittsgeschwindigkeit
        var formattedTime = convertMinutesToTime(estimatedDuration);
        commuteDuration.innerText = `Durchschnittliche Fahrtdauer (basierend auf 50 km/h): ${formattedTime}`;

        hiddenCommuteDuration.value = estimatedDuration.toFixed(2);

    } else {
        commuteDuration.innerText = `Durchschnittliche Fahrtdauer (basierend auf 50 km/h): 0 Minuten`;
    }
}

var loadingInterval;


async function calculateOldDistance() {
    var oldFromLocation = document.getElementById("oldFromLocation").value;
    var oldToLocation = document.getElementById("oldToLocation").value;
    var oldCommuteDistance = document.getElementById("oldCommuteDistance");
    var oldCommuteDuration = document.getElementById("oldCommuteDurationAutomatically");
    var oldHiddenCommuteDistanceField = document.getElementById("hiddenOldCommuteDistance");
    var oldHiddenFullCommuteDistance = document.getElementById("hiddenOldFullCommuteDistance");
    var oldHiddenCommuteDuration = document.getElementById("hiddenOldCommuteDuration");
    var oldHiddenCommuteDurationSeconds = document.getElementById("hiddenOldCommuteDurationSeconds");
    var oldRoundTripDistance = document.getElementById("oldRoundTripDistance");
    var oldLoadingText = document.getElementById("oldLoadingText");
    

    if (!oldFromLocation || !oldToLocation) {
        console.error("Bitte sowohl Start- als auch Zieladresse eingeben.");
        return;
    }
    try {
        
        oldLoadingText.style.display = "flex";
        startLoadingAnimation();
        const oldResponse = await fetch(`/api/distanceApi/oldCalculateDistance?fromLocation=${encodeURIComponent(oldFromLocation)}&toLocation=${encodeURIComponent(oldToLocation)}`);


        if (!oldResponse.ok) {
            console.error("Fehler bei der Anfrage:", oldResponse.status, oldResponse.statusText);
            return;
        }
        const oldData = await oldResponse.json();
       
        // Daten von der API empfangen und in das Formular einfügen
        if (oldData.oldDistance && oldData.oldFullDistance) {

            let oldDistanceValue = oldData.oldDistance.toFixed(2);
            let oldFullDistanceValue = oldData.oldFullDistance.toFixed(2);
            
            oldCommuteDistance.value = oldDistanceValue;

            oldRoundTripDistance.innerText = `${oldFullDistanceValue} Kilometer`;

            oldHiddenCommuteDistanceField.value = oldDistanceValue;
            oldHiddenFullCommuteDistance.value = oldFullDistanceValue;
            
        } else {
            console.log("Keine Distanz gefunden");
        }

        if (oldData.oldDuration) {
            oldCommuteDuration.innerText = `Durchschnittliche Fahrdauer: ${oldData.oldDuration} Minuten und ${oldData.oldDurationSeconds} Sekunden`;
            oldCommuteDuration.style.display = "block";
            //oldHiddenCommuteDistanceField.value = oldData.oldDistance.toFixed(2);
            oldHiddenCommuteDuration.value = oldData.oldDuration;
            oldHiddenCommuteDurationSeconds.value = oldData.oldDurationSeconds;
        } else {
            console.log("Keine Dauer gefunden");
        }
    } catch (error) {
        console.error("Fehler bei der Berechnung der Distanz:", error);
    } finally {
        oldLoadingText.style.display = "none";
        stopLoadingAnimation();
    }
}



async function calculateDistance() {
    var fromLocation = document.getElementById("fromLocation").value;
    var toLocation = document.getElementById("toLocation").value;
    var commuteDistance = document.getElementById("commuteDistance");
    var commuteDuration = document.getElementById("commuteDurationAutomatically");
    var hiddenCommuteDistanceField = document.getElementById("hiddenCommuteDistance");
    var hiddenCommuteDuration = document.getElementById("hiddenCommuteDuration");
    var hiddenCommuteDurationSeconds = document.getElementById("hiddenCommuteDurationSeconds");
    var loadingText = document.getElementById("loadingText");
    var hiddenFullCommuteDistance = document.getElementById("hiddenFullCommuteDistance");
    var roundTripDistance = document.getElementById("roundTripDistance");



    if (!fromLocation || !toLocation) {
        console.error("Bitte sowohl Start- als auch Zieladresse eingeben.");
        return;
    }
    try {
        loadingText.style.display = "flex";
        startLoadingAnimation();
        const response = await fetch(`/api/distanceApi/calculateDistance?fromLocation=${encodeURIComponent(fromLocation)}&toLocation=${encodeURIComponent(toLocation)}`);

        if (!response.ok) {
            console.error("Fehler bei der Anfrage:", response.status, response.statusText);
            return;
        }

        const data = await response.json();

        // Empfange die Daten von der API
        if (data.distance && data.fullDistance) {
            let distanceValue = data.distance.toFixed(2);
            let fullDistanceValue = data.fullDistance.toFixed(2);

            commuteDistance.value = distanceValue;
            roundTripDistance.innerText = fullDistanceValue + " Kilometer";

            hiddenCommuteDistanceField.value = distanceValue;  // Einzelstrecke für das Backend
            hiddenFullCommuteDistance.value = fullDistanceValue;  // Doppelte Strecke für das Backend

        } else {
            console.log("Keine Distanze gefunden");
        }

        if (data.duration) {
            commuteDuration.innerText = `Durchschnittliche Fahrdauer: ${data.duration} Minuten und ${data.durationSeconds} Sekunden`;
            commuteDuration.style.display = "block";
            //hiddenCommuteDistanceField.value = data.distance.toFixed(2);
            hiddenCommuteDuration.value = data.duration;
            hiddenCommuteDurationSeconds.value = data.durationSeconds;

        } else {
            console.log("Keine Dauer gefunden");
        }
    } catch (error) {
        console.error("Fehler bei der Berechnung der Distanz:", error);
    } finally {
        loadingText.style.display = "none";
        stopLoadingAnimation();
    }
}




function startLoadingAnimation() {
    var loadingText = document.getElementById("loadingText");
    var dots = 0;
    loadingInterval = setInterval(function () {
        dots = (dots + 1) % 4;
        var dotText = ".".repeat(dots);
        loadingText.textContent = `Berechne Strecke und Dauer, bitte warten${dotText}`;
    }, 500);
}

function stopLoadingAnimation() {
    clearInterval(loadingInterval);
}

// Diese Funktion wird aufgerufen, wenn sich die Getriebeart oder Kraftstofftyp ändert
function handleTransmissionOrFuelChange() {
    hideModalOnGearChange();
}

function toggleExternalNettoInput() {
    var useExternalNetto = document.querySelector('input[name="useExternalNetto"]:checked').value;
    var externalNettoInput = document.getElementById("externalNettoInput");
    var externalOldNettoInput = document.getElementById("externalOldNettoInput");
    var oldBruttoInput = document.getElementById("oldBruttoInput");
    var newBruttoInput = document.getElementById("newBruttoInput");
    var grossSalaryWithChurchTaxes = document.getElementById("calculatedGrossSalaryWithChurchTaxes");
    var grossSalaryWithOutChurchTaxes = document.getElementById("calculatedGrossSalaryWithoutChurchTaxes");
    var calculatedNetto = document.getElementById("calculatedNetSalary");
    var taxClass = document.getElementById("taxClassComp");
    var churchTaxComp = document.getElementById("churchTaxComp");
    var grossAndNetSalary = document.getElementById("grossAndNetSalary");
    var commutingCostsAndDistance = document.getElementById("commutingCostsAndDistance");
    var grossSalaryOldBruttoInput = document.getElementById("grossSalary");
    var newGrossSalaryOldBruttoInput = document.getElementById("newGrossSalary");
    var netSalaryNew = document.getElementById("netSalaryNew");
    var newNetSalaryNew = document.getElementById("newNetSalaryNew");
    var externalNettoValue = document.getElementById("externalNettoValue");
    var externalOldNettoValue = document.getElementById("externalOldNettoValue");



    if (useExternalNetto === 'true') {


        externalNettoInput.style.display = "block";
        externalOldNettoInput.style.display = "block";
        taxClass.style.display = "none";
        churchTaxComp.style.display = "none";
        oldBruttoInput.style.display = "none";
        newBruttoInput.style.display = "none";
        grossSalaryWithOutChurchTaxes.style.display = "none";
        calculatedNetto.style.display = "none";
        netSalaryNew.innerText = "0,00 €";
        newNetSalaryNew.innerText = "0,00 €";
        grossAndNetSalary.textContent = "Nettogehalt";
        commutingCostsAndDistance.textContent = "";
        grossSalaryOldBruttoInput.value = "";
        newGrossSalaryOldBruttoInput.value = "";



    } else {
        externalNettoInput.style.display = "none";
        externalOldNettoInput.style.display = "none";
        externalOldNettoValue.value = "";
        externalNettoValue.value = "";

        oldBruttoInput.style.display = "block";
        newBruttoInput.style.display = "block";
        taxClass.style.display = "block";
        churchTaxComp.style.display = "block";
        calculatedNetto.style.display = "table-row";
        grossAndNetSalary.textContent = "Bruttogehalt";
        



    }
    handleChurchCheckbox();

}

document.addEventListener("DOMContentLoaded", function () {
    toggleExternalNettoInput();
});

document.getElementById('salaryForm').addEventListener('submit', function (event) {
    const firstInvalidField = this.querySelector(':invalid');
    if (firstInvalidField) {
        firstInvalidField.focus();
    }
}, false);



function clearPlaceholder(input) {
    if (input.value === "0" || input.value === "0.00") {
        input.value = "";
    }
}

function setRequired(element, isRequired) {
    if (isRequired) {
        element.setAttribute("required", "true");
    } else {
        element.removeAttribute("required");
    }
}