import { useContext, useEffect, useState } from "react";
import DayView from "./DayView";
import axios from "axios";
import IRegEvent from "./interfaces/RegEvent";
import INREvent from "./interfaces/NREvent";
import "./WeekPlanner.css";
import ICancellation from "./interfaces/Cancellation";
import React from "react";
import IEventsContext from "./interfaces/EventsContext";
import INRReply from "./interfaces/NREventReply";
import { TokenContext } from "./App";
import useAxiosAuth from "./Hooks/UseAxiosAuth"



const daysDisplayNum: number = 7;
function GenerateWeekDays(startDay: Date) {
    const dayArray:Array<Date> = []
    for (let i = 0; i < daysDisplayNum; i++) {
        const date: Date = new Date(startDay)
        date.setDate(date.getDate() + i - 2)
        dayArray.push(date)
    }
    return dayArray
}
function addWeekDays(n: number, dayArray: Array<Date>) {
    const newDayArray = [...dayArray]
    if (n > 0) {
        for (let i = 0; i < n; i++){
            const lastDay: Date = new Date(dayArray[daysDisplayNum - 1])
            const nextDay: Date = new Date(lastDay.setDate(lastDay.getDate() + 1));
            newDayArray.push(nextDay);
            newDayArray.shift();
        }
    }
    else if (n < 0) {
        for (let i = 0; i > n; i--){
            const firstDay: Date = new Date(dayArray[0]);
            const prevDay: Date = new Date(firstDay.setDate(firstDay.getDate() - 1));
            newDayArray.unshift(prevDay);
            newDayArray.pop();
        }
    }
    return newDayArray;
}

export const EventsContext = React.createContext<IEventsContext | null>(null)
function WeekPlanner() {

    const EventsServer = axios.create({
        baseURL: "https://localhost:54249/api/Events"
    })

    const axiosEvents = useAxiosAuth(EventsServer)


    const [WeekDays, setWeekDays]: [Date[], (WeekDays: Date[]) => void] = useState(GenerateWeekDays(new Date()))


    const [regEvents, setRegEvents] = useState<IRegEvent[]>([])
    const [nrEvents, setNREvents] = useState<INREvent[]>([]);
    const [cancellations, setCancellations] = useState<ICancellation[]>([]);
    const [nreReplies, setNREReplies] = useState<INRReply[]>([])

    async function updateEvents(): Promise<void> {

        setRegEvents(await updateRegEvents())

        setNREvents(await updateNREvents())

        setCancellations(await updateCancellations())

        setNREReplies(await updateNREReplies())
    }
    async function updateRegEvents(): Promise<IRegEvent[]> {
        console.log("updating RegEvents")
        let regEvents: Array<IRegEvent> = new Array<IRegEvent>()
        try {
            const response = await axiosEvents.get("/RegEvents")
            regEvents = response.data.map((e: IRegEvent) => ({
                ...e
            }))
        }
        catch (e) {
            console.error(e)
        }
        return regEvents
    }
    async function updateNREvents(): Promise<INREvent[]> {
        console.log("updating NR Events")
        let nrEvents: INREvent[] = new Array<INREvent>()
        try {
            const response = await axiosEvents.get("/NREvents")
            nrEvents = response.data.map((e: INREvent) => ({
                ...e,
                date: new Date(e.date)
            }))
        }
        catch (e) {
            console.error(e)
        }
        return nrEvents
    }
    async function updateCancellations(): Promise<ICancellation[]> {
        console.log("updating Cancellations")
        let cancellations: ICancellation[] = new Array<ICancellation>
        try {
            const response = await axiosEvents.get("/Cancellations")
            cancellations = response.data.map((c: ICancellation) => ({
                ...c,
                date: new Date(c.date)
            }))
        }
        catch (e) {
            console.error(e)
        }
        return cancellations
    }
    async function updateNREReplies(): Promise<INRReply[]> {
        console.log("updating NR Event Replies")
        let replies: INRReply[] = new Array<INRReply>
        try {
            const response = await EventsServer.get("/NREvents/Replies")
            replies = response.data.map((r: INRReply) => ({
                ...r
            }))
        }
        catch (e) {
            console.error(e)
        }
        return replies
    }

    const eventsContextValue: IEventsContext = { regEvents, setRegEvents, nrEvents, setNREvents, cancellations, setCancellations, nreReplies, updateEvents}

    useEffect(() => {
    })


    const [loading, setLoading] = useState(true);
    
    useEffect(() => {
        (async () => {
            try {
                await updateEvents()
            } catch (error) {
                console.log(error);
            }
            finally {
                setLoading(false)
            }
        })();
    }, []);
    function handleNextDayClick() {
        setWeekDays([...addWeekDays(1, WeekDays)]);
    }
    function handlePrevDayClick() {
        setWeekDays([...addWeekDays(-1, WeekDays)]);
    }

    return (
        <EventsContext.Provider value={eventsContextValue }>
            <div className="week-planner">
                <div className="week-label">
                    <button className="pgbutton" type="button" onClick={handlePrevDayClick}>{"<"}</button>
                    <h2>June 2024</h2>
                    <button className="pgbutton" type="button" onClick={handleNextDayClick}>{">"}</button>
                </div>
                <div className="weekday-container">
                {
                    WeekDays.map((day, index) => {
                        return !loading ? <DayView key={index} day={day} /> : <div>LOADING</div>
                        }
                        )
                }
                </div>
            </div>
        </EventsContext.Provider>
    );
}
export default WeekPlanner