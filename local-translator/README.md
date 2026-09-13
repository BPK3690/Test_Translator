# Local AI Translator

This optional service uses Hugging Face MarianMT models for local English and Spanish translation.

## Setup

From the `local-translator` folder:

```powershell
python -m venv .venv
.\.venv\Scripts\Activate.ps1
python -m pip install -r requirements.txt
python -m uvicorn main:app --reload --port 8000
```

The first translation downloads the selected model from Hugging Face and caches it locally.

Enable the local provider in `appsettings.json`:

```json
"LocalTranslator": {
  "Enabled": true,
  "Endpoint": "http://127.0.0.1:8000"
}
```

The current local models support English to Spanish and Spanish to English. Other language pairs automatically use the existing online translator.
