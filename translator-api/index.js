import { translate } from '@vitalets/google-translate-api';
import createHttpProxyAgent from 'http-proxy-agent';
import express from 'express';


const app = express()
console.log('start server')
app.get('/translate', async (req, res) => {
    const text = req.query.text;
    const to = req.query.to ? undefined : 'ja';
    const from = req.query.from ? undefined : 'id';;

    if (text == undefined) {
        res.json({"error": "text is undefined"});
        return;
    }

    console.log(from + ": " + text)
    const agent = createHttpProxyAgent('http://117.54.114.102:80');
    const result = await translate(text, { 
        to: to,
        fetchOptions: { agent },
    });
    res.json({"text": result.text});
    console.log(to + ": " + result.text);
})

app.listen(3000)