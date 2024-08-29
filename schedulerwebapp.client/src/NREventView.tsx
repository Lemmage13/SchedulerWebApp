import { Card, CardHeader, CardActions, Button } from "@mui/material";
import axios from "axios";
import { useContext } from "react";
import BasicMenu from "./BasicMenu.tsx";
import IEventContainer from "./interfaces/EventContainer";
import IEventsContext from "./interfaces/EventsContext.tsx";
import IMenuMethod from "./interfaces/MenuMethod.tsx";
import { EventsContext } from "./WeekPlanner";


interface props {
    eventContainer: IEventContainer;
    disabled: boolean;
}
function NREventView({ eventContainer, disabled }: props) {
    const eventsContext: IEventsContext | null = useContext(EventsContext)


    const optionsMenuItems: IMenuMethod[] = []

    async function handleNREventAccept(): Promise<void> {
        //call api to update reply to accepted state
        try {
            const response = await axios.put(`https://localhost:54249/api/Events/NREvents/Replies/${eventContainer.reply?.id}`, {
                id: eventContainer.reply?.id,
                nonRegularEventId: eventContainer.reply?.nonRegularEventId,
                accepted: true
            })
            console.log(response)
        }
        catch (e) {
            console.error(e)
        }
        eventsContext?.updateEvents()
    }
    async function handleNREventDecline(): Promise<void> {
        //call api to update reply to declined state
        try {
            const response = await axios.put(`https://localhost:54249/api/Events/NREvents/Replies/${eventContainer.reply?.id}`, {
                id: eventContainer.reply?.id,
                nonRegularEventId: eventContainer.reply?.nonRegularEventId,
                accepted: false
            })
            console.log(response)
        }
        catch (e) {
            console.error(e)
        }
        eventsContext?.updateEvents()
    }


    return (
        <Card
            sx={disabled ? { opacity: 0.5 } : { opacity: 1 }} 
        >
            <CardHeader
                title={eventContainer.event.name}
                action={<BasicMenu sx={{ opacity: 1 }} items={optionsMenuItems}></BasicMenu>}
                subheader={eventContainer.event.time}
            >
            </CardHeader>
            <CardActions
                sx={[
                    { justifyContent: "center" }
                ]}
            >
                <Button variant="contained" size="small" title="Accept" disabled={eventContainer.reply?.accepted == true} onClick={handleNREventAccept}>Accept</Button>
                <Button variant="contained" size="small" title="Decline" disabled={eventContainer.reply?.accepted == false} onClick={handleNREventDecline}>Decline</Button>
            </CardActions>
        </Card>
    )
}
export default NREventView