package be.kdg.stemtest.view.fragments.partygame

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.core.os.bundleOf
import androidx.navigation.findNavController
import androidx.recyclerview.widget.RecyclerView
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.StudentAnswer
import be.kdg.stemtest.model.entity.AnswerOption
import be.kdg.stemtest.model.entity.PartyAnswer
import be.kdg.stemtest.model.entity.Statement
import kotlinx.android.synthetic.main.resultaat_stelling_fragment.view.*

class PgResultAdapter(val context: Context?)
    :RecyclerView.Adapter<PgResultAdapter.ResultViewHolder>(){

    private var answers : List<StudentAnswer> = listOf()
        set(answer) {
            field = answer
            notifyDataSetChanged()
        }

    private var partyAnswers : List<PartyAnswer> = listOf()
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



    class ResultViewHolder(view:View): RecyclerView.ViewHolder(view) {
        val statement = view.resultStelling
        val studentAnswer = view.yourAnswer
        val partyAnswer = view.partyAnswer
    }



    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ResultViewHolder {
        val resultStatementView = LayoutInflater.from(parent.context).inflate(R.layout.resultaat_stelling_fragment,parent,false)
        return ResultViewHolder(
            resultStatementView
        )
    }

    override fun getItemCount(): Int {
        if (answers.isNullOrEmpty()){
            return 0
        } else
            return  partyAnswers!!.size
        }


    override fun onBindViewHolder(holder: ResultViewHolder, position: Int) {
        val currStatement = statements[position]
        holder.statement.text = currStatement.text

        val answerOptionsFiltered = answersOptions.filter { i -> i.statementId == position+1 }

        val studentAnswer = answers.find{ a -> a.id==position+1 }
        holder.studentAnswer.text =
            answerOptionsFiltered.find { ao -> ao.id ==studentAnswer!!.chosenAnswerId }!!.opinion

        val partyAnswer = partyAnswers.find{ a -> a.statementId==currStatement.id }
        holder.partyAnswer.text =
            answerOptionsFiltered.find { ao -> ao.id ==partyAnswer!!.chosenAnswerId }!!.opinion

        holder.itemView.setOnClickListener {v ->
            val navController = v.findNavController()
            navController.navigate(R.id.wachtscherm, bundleOf())
        }



    }

    fun setStudentAnswers(answers:List<StudentAnswer>){
        this.answers = answers
        notifyDataSetChanged()
    }

    fun setPartyAnswer(answers:List<PartyAnswer>){
        this.partyAnswers = answers
        notifyDataSetChanged()
    }

    fun setStelling(stellingen:List<Statement>){
        this.statements = stellingen
        notifyDataSetChanged()
    }

    fun setAnswerOptions(answerOptions:List<AnswerOption>){
        this.answersOptions = answerOptions
        notifyDataSetChanged()
    }





}