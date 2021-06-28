package be.kdg.stemtest.view.fragments.custompartygame

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.StudentAnswer
import be.kdg.stemtest.model.entity.AnswerOption
import be.kdg.stemtest.model.entity.Statement
import kotlinx.android.synthetic.main.c_pg_result_rv_stelling_fragment.view.*

class CPgResultAdapter(val context: Context?)
    : RecyclerView.Adapter<CPgResultAdapter.ResultViewHolder>(){

    private var answers : List<StudentAnswer> = listOf()
        set(answers) {
            field = answers
            notifyDataSetChanged()
        }

    private var correctAnswers : List<AnswerOption> = listOf()
        set(answers) {
            field = answers
            notifyDataSetChanged()
        }

    private var statements : List<Statement> = listOf()
        set(statement) {
            field = statement
            notifyDataSetChanged()
        }

    private var answersOptions : List<AnswerOption> = listOf()
        set(answerOptions) {
            field = answerOptions
            notifyDataSetChanged()
        }



    class ResultViewHolder(view: View): RecyclerView.ViewHolder(view) {
        val statements = view.resultStellingCPg
        val studentAnswer = view.yourAnswerCPg
        val correctAnswer = view.correctAnswerCPg
    }



    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ResultViewHolder {
        val resultaatStellingView = LayoutInflater.from(parent.context).inflate(R.layout.c_pg_result_rv_stelling_fragment,parent,false)
        return ResultViewHolder(
            resultaatStellingView
        )
    }

    override fun getItemCount(): Int {
        if (answers.isNullOrEmpty()){
            return 0
        } else
            return  correctAnswers!!.size
    }


    override fun onBindViewHolder(holder: ResultViewHolder, position: Int) {
        val currStelling = statements[position]
        holder.statements.text = currStelling.text

        val answerOptionsFiltered = answersOptions.filter { i -> i.statementId == position+1 }

        val studentAnswer = answers.find{ a -> a.id==position+1 }
        holder.studentAnswer.text =
            answerOptionsFiltered.find { ao -> ao.id ==studentAnswer!!.chosenAnswerId }!!.opinion


        for (a in correctAnswers) {
            for (i in answerOptionsFiltered){
                if (a.id==i.id){
                    holder.correctAnswer.text = a.opinion
                }
            }
        }


    }





    fun setStudentAnswers(answers:List<StudentAnswer>){
        this.answers = answers
        notifyDataSetChanged()
    }

    fun setCorrectAnswer(answers:List<AnswerOption>){
        this.correctAnswers = answers
        notifyDataSetChanged()
    }

    fun setStatement(stellingen:List<Statement>){
        this.statements = stellingen
        notifyDataSetChanged()
    }

    fun setAnswerOptions(answerOptions:List<AnswerOption>){
        this.answersOptions = answerOptions
        notifyDataSetChanged()
    }





}