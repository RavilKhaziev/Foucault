import string
from fastapi import FastAPI
from fastapi import Request
from typing import List, Dict
from pydantic import BaseModel
from threading import Thread
import time
import uvicorn
from langchain.prompts import load_prompt
from langchain.chains.summarize import load_summarize_chain
from langchain.chat_models.gigachat import GigaChat
from langchain.document_loaders import TextLoader
from langchain_core.prompts import PromptTemplate
from langchain.docstore.document import Document
import requests
import json 

app = FastAPI()

api_ip = "http://cybernadzor:8080/api/AI/test"


class SummarizationModel:
    def __init__(self, token : str, question : str):
        
        text_prompt = """
        Студент ответил в анкете на вопрос:
        """ + question + """
        Ответ студента:{text}. 
        Выдели основные темы ответа и пронумеруй их
        Например:
        Ответ студента:
        В общежитии нет дверей. Также сломаны окна. 
        Темы:
        1. Отсутсвие дверей
        2. Сломаны окна
        Темы:
        """

        self.model = GigaChat(credentials=token, verify_ssl_certs=False)

        self.prompt = PromptTemplate(template=text_prompt, input_variables=["text"])

        self.chain = load_summarize_chain(self.model, chain_type="map_reduce", map_prompt=self.prompt, combine_prompt=self.prompt)

    def get_text_from_arr(self, arr_text : List[str]):
        res_text = ""

        for txt in arr_text:
            res_text += txt.strip()

        return res_text

    def run(self, text):
        doc = [Document(page_content=text, metadata={"source": "local"})]

        res = self.chain.run(doc)

        return res



token = "ZDBlOTIxYzktNmVjYS00ZWZlLThiYTQtYTg1YmNlMTg5OGZiOjc2YWE2ZmZmLWExZTUtNGZmNS05YTE3LTNmM2Y1N2ZkZmE4Mw=="




class MyData(BaseModel):
    ik : str
    answers: List[str]
    question : str

def process(data: MyData):
    id = data.iK
    text_arr = data.answers
    question = data.question
    
    model = SummarizationModel(token, question)
    
    text = model.get_text_from_arr(text_arr)

    result = model.run(text)

    answer = {"IK" : id, "result" : result}

    res = requests.post(url=f"{api_ip}", json=answer)

    print(res)

@app.post("/send")
def summ_data(data: MyData):
    task = Thread(target=process, args=(data,))
    task.start()
    print("I started your task")
    return {'data' : "work_start"}



def plug_fun(id):
    time.sleep(8)
    res = requests.post(url=f"{api_ip}", json={"IK" : id, "result" :"Вывод"})


@app.post("/plug")
async def plug( request : Request):
    print((await request.json()))
    task = Thread(target=plug_fun, args=((await request.json())['ik'],))
    task.start()
    return {'data' : "work_start"}



if __name__ == '__main__':
    uvicorn.run(app, host="0.0.0.0", port=8000)