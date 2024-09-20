import { useContext } from "react"
import RegEventView from "./RegEventView.tsx"
import {EventsContext } from "./WeekPlanner"
import "./WeekPlanner.css"
import IEventContainer from "./interfaces/EventContainer.tsx"
import IEventsContext from "./interfaces/EventsContext.tsx"
import { sameDay } from "./Tools/DateTools.tsx"
import NREventView from "./NREventView.tsx"
import { IconButton } from "@mui/material"
import { AddCircleOutline } from "@mui/icons-material"
import AddEventDialogue from "./AddEventDialogue.tsx"
import { useState } from "react"
interface DayProps {
    day: Date;
}
const daysOfWeek: string[] = ["Sun", "Mon", "Tues", "Weds", "Thurs", "Fri", "Sat"]
function IsPast(day: Date): boolean {
    const now: Date = new Date()
    const currentDay: Date = new Date(now.getFullYear(), now.getMonth(), now.getDate())
    return day < currentDay
}
function DayView({ day }: DayProps) {
    const eventsValues: IEventsContext | null = useContext(EventsContext)

    const regEvents: Array<IEventContainer> = regEventsForDay()
    const nrEvents: IEventContainer[] = nrEventsForDay()

    function regEventsForDay() {
        const dayRegEvents: Array<IEventContainer> = [];
        if (eventsValues != null) {
            eventsValues.regEvents.forEach((e) => {
                if (day.getDay() === e.dayOfWeek) {
                    const eC: IEventContainer = { event: e };
                    eventsValues.cancellations.forEach((c) => {
                        if (sameDay(c.date, day)) {
                            eC.cancellation = c;
                        }
                    });
                    dayRegEvents.push(eC);
                }
            });
        }
        return dayRegEvents;
    }
    function nrEventsForDay() {
        const dayNREvents: IEventContainer[] = [];
        if (eventsValues != null) {
            eventsValues.nrEvents.forEach((e) => {
                if (sameDay(day, e.date)) {
                    const eC: IEventContainer = { event: e }
                    //access accept objects
                    eventsValues.nreReplies.forEach((r) => {
                        if (r.nonRegularEventId === e.id) {
                            eC.reply = r
                        }
                    })
                    dayNREvents.push(eC)
                }
            })
        }
        return dayNREvents
    }
    const [addEventDialogueOpen, setAddEventDialogueOpen] = useState(false);
    function handleOpenAddEventDialogue() {
        setAddEventDialogueOpen(true)
    }
    function handleOpenEventDialogueClose() {
        eventsValues?.updateEvents()
        setAddEventDialogueOpen(false)
    }
    return (
        <div className="day">
            <h3>{daysOfWeek[day.getDay()]}</h3>
            <h3>{day.getDate().toString()}</h3>
            {
                regEvents.map((e, index) => {
                    return <RegEventView key={index} eventContainer={e} disabled={IsPast(day)} day={day}></RegEventView>
                })
            }
            {
                nrEvents.map((e, index) => {
                    return <NREventView key={index} eventContainer={e} disabled={IsPast(day)}></NREventView>
                })
            }
            <IconButton onClick={handleOpenAddEventDialogue }>
                <AddCircleOutline/>
            </IconButton>
            <AddEventDialogue open={addEventDialogueOpen} date={ day } onClose={handleOpenEventDialogueClose}></AddEventDialogue>
        </div>
    );
}

export default DayView;