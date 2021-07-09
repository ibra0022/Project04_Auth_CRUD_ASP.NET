import React, {useEffect, useState} from 'react';
import axios from "axios";

function Info(props) {

    const [element, setElement] = useState({});

    useEffect(() => {
        if (props.which == 1){
            console.log(props.id)
            axios.get(`https://localhost:5001/api/movies/${props.id}`)
                .then(res => {
                    const list = res.data;
                    setElement(list);
                }).catch(err => {
                console.log(err)
            })
        } else if (props.which == 2) {
            axios.get(`https://localhost:5001/api/tvshows/${props.id}`)
                .then(res => {
                    const list = res.data;
                    setElement(list);
                }).catch(err => {
                console.log(err)
            })
        }
        return () => {
        }
    }, [])
    
    
    return (
        <div className="container fade-me">
            <div className="main-body">
                <nav
                    aria-label="breadcrumb"
                    className="main-breadcrumb"

                >
                    <ol className="breadcrumb rad" style={{ backgroundColor: "#191919" }}>
                        <h1 style={{ marginLeft: "20px" }}>{element? element.title : null}</h1>
                    </ol>
                </nav>

                <div className="row gutters-sm rad" >
                    <div className="col-md-4 mb-3 rad">
                        <div className="card rad" style={{ backgroundColor: "#191919" }}>
                            <div className="card-body rad" style={{ backgroundColor: "#191919" }}>
                                <div className="d-flex flex-column align-items-center text-center">
                                    {element? <img src={element.poster} className="rad" height="450px"/> : null}
                                    <div className="mt-3">
                                        {/*<h4>{props.user.name}</h4>*/}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="col-md-8 rad">
                        <div className="card mb-3 rad" style={{ backgroundColor: "#191919" }}>
                            <div className="card-body rad" style={{ backgroundColor: "#191919" }}>
                                <div className="row">
                                    <div className="col-sm-4">
                                        <h6 className="mb-0">Title</h6>
                                    </div>
                                    <div className="col-sm-8 text-secondary">
                                        {element? element.title : null}
                                    </div>
                                </div>
                                <hr />
                                <div className="row">
                                    <div className="col-sm-4">
                                        <h6 className="mb-0">Date</h6>
                                    </div>
                                    <div className="col-sm-8 text-secondary">
                                        {element? element.date : null}
                                    </div>
                                </div>
                                <hr />
                                <div className="row">
                                    <div className="col-sm-4">
                                        <h6 className="mb-0">Summary</h6>
                                    </div>
                                    <div className="col-sm-8 text-secondary">
                                        {element? element.summary : null}
                                    </div>
                                </div>
                                <hr />
                                <div className="row">
                                    <div className="col-sm-4">
                                        <h6 className="mb-0">Rating</h6>
                                    </div>
                                    <div className="col-sm-8 text-secondary">
                                        {element? element.rating : null}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Info;