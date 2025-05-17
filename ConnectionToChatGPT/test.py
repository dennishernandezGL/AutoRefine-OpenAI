from openai import OpenAI
import json

#API_KEY = ""

# Configurar los headers
headers = {
    "Content-Type": "application/json",
    "Authorization": f"Bearer {API_KEY}"
}

#Crea una instancia del cliente OpenAI utlizando una clave de API proporcionada 
client = OpenAI(
  api_key=API_KEY
)

def prompt_generator(information):
    #Generacion de la respuesta atraves del API de chat GPT
    prompt = """
    I want you to act as a prompt generator for other programs.
    The structure you will receive is as follows:
    "prompt": "prompt text",
    "context": "prompt context",
    "output_format": "text",
    "language": "prompt language",
    "tone": "content tone",
    "length": "100 - 300",
    "keywords": keywords
    I want you to return the response using exactly the same format.
    """
    #Concatenamos toda la informacion
    full_prompt = prompt + "\n" + str(information)
    
    response = client.chat.completions.create(
        model="gpt-4o",
        messages=[
            {"role": "user", "content": full_prompt}
        ]
    )
    
     # Obtener el texto generado por el modelo
    output_text = response.choices[0].message.content.strip()
    
    print(output_text)

    # # Estructura de datos para guardar en JSON
    # json_output = json.loads(output_text)
    
    # # Guardar como archivo JSON en la carpeta actual
    # with open("prompt_output.json", "w") as archivo:
    #     json.dump(json_output, archivo, indent=4)

def request_data():
    print("===== Generador de promts para Chatgpt =====\n")
    #Descripcion del prompt
    prompt = input("Describe brevemente que quiere que realize este prompt\n")
    
    #Objetivo del prompt
    context = input("Cual es el objetivo del promt\n")
    
    #lenguaje
    language = input("Por defecto el lenguaje de salida es ingles para un mayor rendimiento \nDeseas cambiarlo por Spanish: si o no\n")
    if language.lower() == "no":
        language = "en"
    elif language.lower() == "si":
        language = "es"
    else:
        print("Opcion invalida, por defecto queda en ingles")
        language = "en"
        
    #tono del prompt
    tone = input("El tono para este promt es: profesional, Deseas cambiarlo Si o No: \n")
    if tone.lower() == "no":
        tone = "profesional"
    elif tone.lower() == "si":
        tone = input("Digite el tono que deseas: ")
    else:
        print("Opcion no valida, tono por defecto profesional")
        tone = "profesional"
    
    #Palabras claves 
    keywords = input("Quires agregar palabras claves, pueden ayudar a optimizar el prompt: si o no\n")
    if keywords == "si":
        keywords = input("Digite las palabras separadas por comas: \n")
    else:
        keywords = " "
    
    information = {
        "promt": prompt,
        "context": context,
        "output_format": "JSON",
        "language": language,
        "tone": tone,
        "length": "100 - 300",
        "keywords": keywords
    }
    prompt_generator(information)
    
        
if __name__ == "__main__":
    pass
    request_data()
    # prompt_final = generar_prompt_mejorado(datos_usuario)
    # print("\n=== Prompt Generado ===")
    # print(prompt_final)