import { translate } from '@vitalets/google-translate-api';
import createHttpProxyAgent from 'http-proxy-agent';
import express from 'express';

var proxyList = [
    'http://103.106.219.144:8080', 
    'http://117.54.114.102:80'
];

var agent1 = createHttpProxyAgent(proxyList[0]);
var agent2 = createHttpProxyAgent(proxyList[1]);
var proxyAgent = {
    Number: 1,
    Agent: agent1
};

function RollAgent() {
    if (proxyAgent.Number == 1) {
        proxyAgent.Number = 2;
        proxyAgent.Agent = agent2;
    } else if (proxyAgent.Number == 2) {
        proxyAgent.Number = 1;
        proxyAgent.Agent = agent1;
    }
}

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
    for (var retry = 0; retry < 2; i++){
        try {
            const result = await translate(text, { 
                to: to,
                fetchOptions: { proxyAgent: proxyAgent.Agent },
            });
            res.json({"text": result.text});
            console.log(to + ": " + result.text);
            return;
        } catch (e) {
            if (e.name == "TooManyRequestsError") {
                RollAgent();
            }

            if (retry == 1) {
                res.status(500).json({"error":result.text})
                break;
            }
            
            console.log("error" + ": " + e.name);
            return;
        }
    }
})

app.listen(3000)