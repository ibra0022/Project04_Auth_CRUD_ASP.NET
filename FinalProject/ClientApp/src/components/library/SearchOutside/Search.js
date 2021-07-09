import React, {useEffect, useState} from 'react';
import axios from "axios";
import {useHistory} from "react-router-dom";

export default function Search(props){

    const [title, setTitle] = useState("");
    const [List, setList] = useState([]);
    const [show, setShow] = useState(false);

    const history = useHistory();
    

    const serach = () => {
        if (props.which == 1) {
            axios.get(`https://localhost:5001/api/search/movie/${title}`, {
                withCredentials: true
            })
                .then(res => {
                    console.log(res.data)
                    const list = res.data;
                    setList(list);
                    setShow(true)
                }).catch(err => {
                console.log("Error")
            })
        }
        else if (props.which == 2) {
            axios.get(`https://localhost:5001/api/search/tvshow/${title}`, {
                withCredentials: true
            })
                .then(res => {
                    console.log(res.data)
                    const list = res.data;
                    setList(list);
                    setShow(true)
                }).catch(err => {
                console.log("Error")
            })
        }
    }

    const titleOnChange = (event) => {
        setTitle(event.target.value);
    }

    const addToDatabase = (element) => {
        if (props.which == 1) {
            axios.post("https://localhost:5001/api/movies", {
                title: element.title,
                date: element.date,
                summary: element.summary,
                rating: element.rating,
                poster: element.poster,
            }, {
                withCredentials: true
            }).then(res => {
                console.log(res);
                history.push("/movies");
            }).catch(err => console.log(err));
        }
        else if (props.which == 2) {
            axios.post("https://localhost:5001/api/tvshows", {
                title: element.title,
                date: element.date,
                summary: element.summary,
                rating: element.rating,
                poster: element.poster,
            }, {
                withCredentials: true
            }).then(res => {
                console.log(res);
                history.push("/tv-shows");
            }).catch(err => console.log(err));
        }
    }

    const renderList = (List) => {
        return (
            <div className="row row-cols-1 row-cols-md-4 g-4 fade-me">
                {List.map((element, index) =>
                    <div className="col " key={index} >
                        <div className="card hoverMe" style={{ borderRadius: "30px"}}>
                            <img src={element.poster} className="card-img-top" alt="..." style={{ borderRadius: "30px 30px 0px 0px"}}  />
                            <div className="card-body">
                                <h5 className="card-title" style={{paddingBottom: "20px"}} >{element.title}</h5>
                                <div className="card-text" style={{textAlign: "right"}}>
                                    <i className="fas fa-plus fa-lg myActionBtn" onClick={() => addToDatabase(element)}></i>
                                </div>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        );
    }

    let contents = show
        ? renderList(List)
        : null;
    
    let header;
    if (props.which == 1) {
        header = "Search for Movie"
    } else if (props.which == 2) {
        header = "Search for TV Show"
    }
        
    return (
        <div className="fade-me">
            <h1 className="neonText" style={{textAlign: "center", width: "700px"}}>{header}</h1>
            <div
                className="card rad"
                style={{
                    width: "22rem",
                    margin: "0 auto",
                    marginTop: "3em",
                    marginBottom: "3em",
                    textAlign: "left",
                    padding: "2em",
                    backgroundColor: "#191919"
                }}
            >
                <div className="mb-3">
                    <input
                        type="text"
                        className="form-control"
                        id="search"
                        aria-describedby="titleHelp"
                        onChange={titleOnChange}
                    />
                </div>
                <button type="submit" onClick={serach} className="btn " style={{
                    marginTop: "1em",
                    backgroundColor: "#680000",
                    color: "white"
                }}>
                    Search
                </button>

            </div>
            {contents}
        </div>
    );
}

