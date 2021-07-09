import React, {useEffect, useState} from 'react';
import {useHistory} from 'react-router-dom';

import axios from "axios";

export default function Edit(props) {

    const [element, setElement] = useState({});
    const [title, setTitle] = useState("");
    const [date, setDate] = useState("");
    const [summary, setSummary] = useState("");
    const [rating, setRating] = useState(0);
    const [poster, setPoster] = useState("");
    const [loading, setLoading] = useState(true);
    
    

    useEffect(() => {
        console.log("edit", props);
        if (props.which == 1)
        {
            console.log("movie")
            console.log(props)
            
            axios.get(`https://localhost:5001/api/movies/${props.id}`)
                .then(res => {
                    const list = res.data;
                    setElement(list);
                    setTitle(list.title)
                    setDate(list.date)
                    setSummary(list.summary)
                    setRating(list.rating)
                    setPoster(list.poster)
                    setLoading(false)
                    console.log(res.data)
                }).catch(err => {
                console.log(err)
            })
        }
        else if (props.which == 2)
        {
            console.log("tv")
            axios.get(`https://localhost:5001/api/tvshows/${props.id}`)
                .then(res => {
                    const list = res.data;
                    setElement(list);
                    setTitle(list.title)
                    setDate(list.date)
                    setSummary(list.summary)
                    setRating(list.rating)
                    setPoster(list.poster)
                    setLoading(false)
                    console.log(res.data)
                }).catch(err => {
                console.log(err)
            })
        }
        
        return () => {

        }
    }, [])

    const history = useHistory();

    
    const titleOnChange = (event) => {
        setTitle(event.target.value)
    }

    const dateOnChange = (event) => {
        setDate(event.target.value);
    }

    const summaryOnChange = (event) => {
        setSummary(event.target.value);
    }

    const ratingOnChange = (event) => {
        let rating = parseInt(event.target.value, 10)
        if (rating > 10 || rating < 1 || isNaN(rating)){
            rating = 5
        }
        setRating(rating)
    }

    const posterOnChange = (event) => {
        setPoster(event.target.value)
    }

    const submit = () => {
        if (props.which == 1) {
            axios.put(`https://localhost:5001/api/movies/${props.id}`, {
                title: title,
                date: date,
                summary: summary,
                rating: rating,
                poster: poster,
            }, {
                withCredentials: true,
            }).then(res => {
                console.log(res);
                history.push("/movies");

            }).catch(err => console.log(err));
        }
        else if (props.which == 2) {
            axios.put(`https://localhost:5001/api/tvshows/${props.id}`, {
                title: title,
                date: date,
                summary: summary,
                rating: rating,
                poster: poster,
            }, {
                withCredentials: true,
            }).then(res => {
                console.log(res);
                history.push("/tv-shows");

            }).catch(err => console.log(err));
        }
    }

    return (
        <div className="fade-me">
            <h1 className="neonText" style={{textAlign: "center"}}>Edit Movie</h1>
            <div
                className="card rad"
                style={{
                    width: "40rem",
                    margin: "0 auto",
                    marginTop: "5em",
                    textAlign: "left",
                    padding: "3em",
                    backgroundColor: "#191919"
                }}
            >
                <div className="mb-3">
                    <label htmlFor="title" className="form-label">
                        Title
                    </label>
                    <input
                        type="text"
                        defaultValue={element? element.title : "" || ""}
                        className="form-control"
                        id="title"
                        aria-describedby="titleHelp"
                        onChange={titleOnChange}

                    />
                </div>
                <div className="mb-3">
                    <label htmlFor="date" className="form-label">
                        Date
                    </label>
                    <input
                        type="date"
                        defaultValue={element? element.date : "" || ""}
                        className="form-control"
                        id="date"
                        onChange={dateOnChange}
                    />
                </div>
                <div className="mb-3">
                    <label htmlFor="summary" className="form-label">
                        Summary
                    </label>
                    <textarea
                        rows="3"
                        defaultValue={element? element.summary : "" || ""}
                        className="form-control"
                        id="summary"
                        onChange={summaryOnChange}
                    />
                </div>
                <div className="mb-3">
                    <label htmlFor="rating" className="form-label">
                        Rating
                    </label>
                    <input
                        type="number"
                        className="form-control"
                        defaultValue={element? element.rating : "" || ""}
                        id="rating"
                        min="1"
                        max="10"
                        onChange={ratingOnChange}
                    />
                </div>
                <div className="mb-3">
                    <label htmlFor="poster" className="form-label">
                        Poster
                    </label>
                    <input
                        type="text"
                        defaultValue={element? element.poster : "" || ""}
                        className="form-control"
                        id="poster"
                        onChange={posterOnChange}
                    />
                </div>
                <button type="submit" onClick={submit} className="btn" style={{
                    marginTop: "3em",
                    backgroundColor: "#680000",
                    color: "white"
                }}>
                    Edit
                </button>
            </div>
        </div>
    );
    // }
}
