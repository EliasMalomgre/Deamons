package be.kdg.stemtest.view.fragments

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.*
import kotlinx.android.synthetic.main.partyinfo_recycler_fragment.view.*

class PartyInfoAdapter (val context: Context?)
    : RecyclerView.Adapter<PartyInfoAdapter.PartyInfoViewHolder>(){

    private var partyAns: List<PartyAnswer> = listOf()
        set(result) {
            field = result
            notifyDataSetChanged()
        }
    fun setPartyAnsData(results: List<PartyAnswer>) {
        this.partyAns = results
        notifyDataSetChanged()
    }

    private var answers: List<StudentAnswer> = listOf()
        set(result) {
            field = result
            notifyDataSetChanged()
        }
    fun setAnswerData(results: List<StudentAnswer>) {
        this.answers = results
        notifyDataSetChanged()
    }


    private var statements: List<Statement> = listOf()
        set(result) {
            field = result
            notifyDataSetChanged()
        }
    fun setStatementData(results: List<Statement>) {
        this.statements = results
        notifyDataSetChanged()
    }

    private var answersOptions: List<AnswerOption> = listOf()
        set(answerOption) {
            field = answerOption
            notifyDataSetChanged()
        }

    fun setAnswerOptionsData(answerOptions: List<AnswerOption>) {
        this.answersOptions = answerOptions
        notifyDataSetChanged()
    }




    class PartyInfoViewHolder(view: View): RecyclerView.ViewHolder(view) {
        val statement = view.tvPartyInfoStatement
        val partyAgree = view.tvPartyInfoAgree
        val agree = view.tvPartyInfoYourAgree
        val partyArg = view.tvPartyInfoArgument
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): PartyInfoViewHolder {
        val partyInfoView = LayoutInflater.from(parent.context).inflate(R.layout.partyinfo_recycler_fragment,parent,false)
        return PartyInfoViewHolder(
            partyInfoView
        )
    }

    override fun getItemCount(): Int {
        if (statements.isNullOrEmpty()) {
            return 0
        } else
            return statements!!.size
    }

    override fun onBindViewHolder(holder: PartyInfoViewHolder, position: Int) {
        val stat = statements[position]

        holder.statement.text = stat.text


        val answerOptionsFiltered = answersOptions.filter { i -> i.statementId == position+1 }

        val answer = answers.find{ a -> a.id==position+1 }
        holder.agree.text=
            answerOptionsFiltered.find { ao -> ao.id ==answer!!.chosenAnswerId }!!.opinion

        val partyAnswer = partyAns.find{ a -> a.statementId==stat.id }
            holder.partyAgree.text =
            answerOptionsFiltered.find { ao -> ao.id ==partyAnswer!!.chosenAnswerId }!!.opinion

        holder.partyArg.text=partyAnswer?.argument
    }
}