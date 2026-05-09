

## Umbraco hicks up
- Mange hick-ups med at få agents til at kører grunde permissions
	- Husk at tilføje i mcp.json eller bare kopier den
- Umbraco Client Id og Secret kommer fra en API bruger man skal oprette i BackOffice, samt give adgang til ALT - umiddelbart indtil videre for ikke at få 401 på nogle MCP requests.
- Umbraco MCP skal startes hver gang VS lukkes (Toppen af CoPolit chat window på tandhjul ikonet. Find Umbraco MCP -> højre klik og start - Muligvis! er det ikke krævet og den starter måske selv når man får agenten til at køre)
- Umbraco MCP har ikke default adgang til alle features, i Copilot chat window try to lille ikon med tooltip "Configure Tools" lige til højre for knappen hvor man skal vælge AI Modeller
- Doctypes fik ikke oprettet input felter begge gange, men efter en promp om de manglede kom de fint på. Hvis det forsætter så lave en Instruction regl om altid at tjekke efter de normal "skulle" være oprettet om de faktisk er, eller kør det igen.
- For så vidt muligt undgå at Agent åbner og logger ind for at klikke rundt i UI, det kan godt messe lidt op og ende med at stå og lave ingen ting / event.
	- Så heller output steps som udvilker lige hurtigt klarer i punkform. Og hvis muligt for implementing få Agenten til at sætte det på en ToDo til senere så den kan gå videre med næste feature hvis muligt
     - Nok en god ting at tilføje til copilot-instructions.md medmindre det kan stabliseres
- Request data med query {} 
	- Powershell script oprettet og entry i RULEBOOK.md om at invoke det script efter behov
- Nogle request CoPilot laver mod Umbraco via PowerShell for at lave smoke tests har det med at fryze eller bare ikke komme videre virker det til. Stop Agent og skriv "retry" eller "are we stuck" har det med at få den til at komme videre

Brug den nu tilrettet og opdateret struktur i ".github/*" til nye projekter og prompt blot at den lige skal wipes for features etc. og prompt så features agenten opretter.

### Token observation
uden at have holdt helt æje med det før meget sent i processen, så jeg token context var meget højt oppe, det har jeg ikke oplevet med det andet "implementering pattern" jeg kørte før Multi Agent mode. 

### Start Multi Agent mode
Vælg "architect" agent fra dropdown, og paste så prompts i rækkefølgen

	> copilot --agent architect

	> Start execution using .github/ai/RUNBOOK.md and continue until all tasks are complete.

### Database eller Process låst?

Kør dette for at nakke processen i PowerShell

    > $ports = 44304,59112

    > $pids = Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue | >>   Where-Object { ports -contains $_.LocalPort } | >>   Select-Object -ExpandProperty OwningProcess -Unique
    
    > if ($pids) { Stop-Process -Id $pids -Force }

    > Get-Process iisexpress -ErrorAction SilentlyContinue | Stop-Process -Force