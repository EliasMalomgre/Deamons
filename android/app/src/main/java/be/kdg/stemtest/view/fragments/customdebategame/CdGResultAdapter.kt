package be.kdg.stemtest.view.fragments.customdebategame

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.core.os.bundleOf
import androidx.navigation.findNavController
import androidx.recyclerview.widget.RecyclerView
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.*
import kotlinx.android.synthetic.main.c_dg_result_rv_stelling_fragment.view.*

class CdGResultAdapter(val context: Context?)
    : RecyclerView.Adapter<CdGResultAdapter.CDgResultViewHolder>(){


    private var statements: List<Statement> = listOf()
        set(statement) {
            field = statement
            notifyDataSetChanged()
        }


    private var answers: List<StudentAnswer> = listOf()
        set(answer) {
            field = answer
            notifyDataSetChanged()
        }

    private var answersOptions : List<AnswerOption> = listOf()
        set(answerOptions) {
            field = answerOptions
            notifyDataSetChanged()
        }


    class CDgResultViewHolder(view: View): RecyclerView.ViewHolder(view) {
        val statement = view.CDg_result_statement
        val answer = view.CDg_result_answer
    }

    override fun onBindViewHolder(holder: CDgResultViewHolder, position: Int) {
        val statement = statements[position]
        holder.statement.text=statement.text

        val answerOptionsFiltered = answersOptions.filter { i -> i.statementId == position+1 }

        val studentAnswer = answers.find{ a -> a.id==position+1 }
        holder.answer.text =
            answerOptionsFiltered.find { ao -> ao.id ==studentAnswer!!.chosenAnswerId }!!.opinion


        holder.itemView.setOnClickListener { v ->
            val navController = v.findNavController()
            val bundle = bundleOf("index" to position)
            navController.navigate(R.id.pieFragment,bundle)
        }
    }


    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): CDgResultViewHolder {
        val CDgResultView = LayoutInflater.from(parent.context).inflate(R.layout.c_dg_result_rv_stelling_fragment,parent,false)
        return CDgResultViewHolder(
            CDgResultView
        )
    }

    override fun getItemCount(): Int {
        if (statements.isNullOrEmpty()){
            return 0
        } else
            return  statements!!.size
    }


    fun setStatementData(statements: List<Statement>){
        this.statements = statements
        notifyDataSetChanged()
    }

    fun setAnswerData(answers: List<StudentAnswer>){
        this.answers = answers
        notifyDataSetChanged()
    }

    fun setAnswerOptionsData(answersOptions: List<AnswerOption>){
        this.answersOptions = answersOptions
        notifyDataSetChanged()
    }


}