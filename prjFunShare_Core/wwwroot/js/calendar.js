const monthDays =document.querySelector(".days");

const date = new Date();

const renderCalendar=()=>{

    const months=[
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December",
    ];
    document.querySelector(".date h1").innerHTML = months[date.getMonth()];
    document.querySelector('.date p').innerHTML =new Date().toDateString();

    date.setDate(1);
    const lastDay = new Date(date.getFullYear(),date.getMonth()+1,0).getDate() //當月最後一天

    const prevLastDay = new Date(date.getFullYear(),date.getMonth(),0).getDate() //上個月最後一天

    const firstDayIndex = date.getDay(); //當月第一天是星期幾

    const lastDayIndex = new Date(date.getFullYear(),date.getMonth()+1,0).getDay() //當月最後一天星期幾
    console.log(lastDayIndex)
    const nextDays = 7-lastDayIndex-1;

    let days ="";
    
    for(let x=firstDayIndex;x>0;x--){
        days+=`<div class="prev-date flip" data-date="${date.getFullYear()}-${date.getMonth()}-${prevLastDay-x+1}">${prevLastDay-x+1}</div>`
    }

    for(let i =1; i<=lastDay;i++){
        if(i===new Date().getDate()&&date.getMonth()===new Date().getMonth()){
            days+=`<div class="today flip" data-date="${date.getFullYear()}-${date.getMonth()+1}-${i}">${i}</div>`;
        }
        else{
            days+=`<div class="flip" data-date="${date.getFullYear()}-${date.getMonth()+1}-${i}">${i}</div>`;
        }
    }

    for(let j=1;j<=nextDays;j++){
        days+=`<div class="next-date flip" data-date="${date.getFullYear()}-${date.getMonth()+2}-${j}">${j}</div>`
    }
    monthDays.innerHTML =days;

      //收合script
        $('.flip').click(function() {
            console.log(this)
            $(".schedule").toggle();
            flipClicked = true;
        });
}

renderCalendar();

    document.querySelector('.prev').addEventListener('click', () => {
        date.setMonth(date.getMonth() - 1);
        renderCalendar();
    });

    document.querySelector('.next').addEventListener('click', () => {
        date.setMonth(date.getMonth() + 1);
        renderCalendar();
    });

  
    


      