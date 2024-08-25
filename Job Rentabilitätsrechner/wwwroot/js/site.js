// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function updateFuelPrice() {
    var fuelType = document.getElementById("fuelType").value;
    var fuelPriceField = document.getElementById("fuelPriceField");
    var fuelPriceLabel = document.getElementById("fuelPriceLabel");
    var fuelInput = document.getElementById("fuelPrice");

    var fuelData = {
        "diesel": { label: "Durchschnittlicher Preis für Diesel (€/Liter):", value: "1.50" },
        "superE10": { label: "Durchschnittlicher Preis für Super E10 (€/Liter):", value: "1.65" },
        "superE5": { label: "Durchschnittlicher Preis für Super E5 (€/Liter):", value: "1.70" },
        "superPlus": { label: "Durchschnittlicher Preis für Super Plus (€/Liter):", value: "1.81" },
        "electricty": { label: "Durchschnittler Preis für Strom (€/kwh):", value: "0.60" }
    };

    if (fuelType in fuelData) {
        fuelPriceField.style.display = "block";
        fuelPriceLabel.textContent = fuelData[fuelType].label;
        fuelInput.value = fuelData[fuelType].value;
    } else {
        fuelPriceField.style.display = "none";
    }
}

document.addEventListener("DOMContentLoaded", function () {
    updateFuelPrice();
});

function calculateNetSalary() {
    var grossSalary = document.getElementById("grossSalary").value;
    var newGrossSalary = document.getElementById("newGrossSalary").value;
    var taxClass = document.getElementById("taxClass").value;
    var churchTax = document.getElementById("churchTax").checked;

    fetch(`/?handler=CalculateNetSalary&grossSalary=${grossSalary}&newGrossSalary=${newGrossSalary}&taxClass=${taxClass}&churchTax=${churchTax}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.text();  // Verwende text() anstelle von json(), um zu sehen, was die Antwort tatsächlich ist
        })
        .then(text => {
            console.log("Response Text: ", text);
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
                document.getElementById("netSalaryNew").innerText = data.netSalary.toFixed(2);
                document.getElementById("newNetSalaryNew").innerText = data.newNetSalary.toFixed(2);
            }
        })
        .catch(error => console.error('Fehler:', error));
}


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

document.addEventListener("DOMContentLoaded", function () {
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
});
