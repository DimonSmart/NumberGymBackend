# NumberGymBackend (retired)

NumberGym no longer performs cloud-based pronunciation analysis. The client uses local multiple-choice exercises and optional device-provided speech recognition/TTS instead.

This repository retains **only a temporary HTTP 410 Gone compatibility endpoint** so a deployment can safely retire the previous public `POST /api/pronunciation/analyze` route without calling or billing Azure Speech.

## Decommissioning

1. Merge and deploy this change to replace the existing public function. Validate that `POST /api/pronunciation/analyze` returns HTTP 410 and does **not** call Azure Speech.
2. Remove `SPEECH_KEY`, `SPEECH_REGION` and `SPEECH_LOCALE` from application settings; revoke the corresponding credentials.
3. Once older app versions are no longer supported, **delete the deployed Azure Function App** and any dedicated Azure Speech resource, or disable the Function App immediately if no compatibility period is required.
4. Archive this repository and remove the deployment workflow after the Azure resources are gone.

**Important:** Removing code from GitHub or merging a pull request does not itself delete previously deployed Azure resources. The endpoint remains publicly reachable until the retired function is deployed or the Function App is disabled.
