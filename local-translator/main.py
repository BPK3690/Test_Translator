from functools import lru_cache

from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from transformers import pipeline

app = FastAPI(title="Local Translator")


class TranslationRequest(BaseModel):
    text: str
    source_language: str
    target_language: str


class TranslationResponse(BaseModel):
    translation: str


@lru_cache(maxsize=2)
def get_pipeline(source_language: str, target_language: str):
    model_name = {
        ("en", "es"): "Helsinki-NLP/opus-mt-en-es",
        ("es", "en"): "Helsinki-NLP/opus-mt-es-en",
    }.get((source_language, target_language))

    if model_name is None:
        raise ValueError("The local model currently supports English and Spanish.")

    return pipeline("translation", model=model_name)


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}


@app.post("/translate", response_model=TranslationResponse)
def translate(request: TranslationRequest) -> TranslationResponse:
    if not request.text.strip():
        raise HTTPException(status_code=400, detail="Text is required.")

    try:
        translator = get_pipeline(request.source_language, request.target_language)
        result = translator(request.text.strip(), max_length=512)
    except ValueError as error:
        raise HTTPException(status_code=400, detail=str(error)) from error

    return TranslationResponse(translation=result[0]["translation_text"])