using GoogleMobileAds.Ump.Api;
using UnityEngine;

public class CMP : MonoBehaviour {
    private void Start() {
        // Set tag for under age of consent.
        // Here false means users are not under age of consent.
        var request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
        };

        ConsentInformation.Update(request, OnConsentInfoUpdated);
        ISHandler.Instance.Init(true);
    }

    private void OnConsentInfoUpdated(FormError consentError) {
        if (consentError != null) {
            Debug.LogError(consentError);
            return;
        }

        ConsentForm.LoadAndShowConsentFormIfRequired(formError =>
        {
            if (formError != null) {
                Debug.LogError(formError);
                return;
            }

            if (ConsentInformation.CanRequestAds()) {
                ISHandler.Instance.Init(true);
            }
        });
    }
}